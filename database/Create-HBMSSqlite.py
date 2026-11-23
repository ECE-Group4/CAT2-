#!/usr/bin/env python3
"""
Cross-Platform Database Generator for HBMS
Converts MS Access schema and synthetic dataset into an SQLite3 database (HBMS.sqlite).
Enables seamless database operations, tests, and queries on Linux, macOS, and Windows.
"""

import os
import re
import sqlite3
import sys

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
SCHEMA_PATH = os.path.join(SCRIPT_DIR, "schema.sql")
DATASET_PATH = os.path.join(SCRIPT_DIR, "synthetic_dataset.sql")
SQLITE_DB_PATH = os.path.join(SCRIPT_DIR, "HBMS.sqlite")

def transform_ddl(sql: str) -> str:
    # Strip full-line comments and inline comments
    text = re.sub(r'--[^\n]*', '', sql)
    # Transform types
    text = re.sub(r'\bAUTOINCREMENT\s+PRIMARY\s+KEY\b', 'INTEGER PRIMARY KEY AUTOINCREMENT', text, flags=re.IGNORECASE)
    text = re.sub(r'\bYESNO\b', 'INTEGER', text, flags=re.IGNORECASE)
    text = re.sub(r'\bCURRENCY\b', 'DECIMAL(12,2)', text, flags=re.IGNORECASE)
    text = re.sub(r'\bLONGTEXT\b', 'TEXT', text, flags=re.IGNORECASE)
    text = re.sub(r'\bDATETIME\s+DEFAULT\s+NOW\(\)', 'DATETIME DEFAULT CURRENT_TIMESTAMP', text, flags=re.IGNORECASE)
    text = re.sub(r'\bDEFAULT\s+TRUE\b', 'DEFAULT 1', text, flags=re.IGNORECASE)
    text = re.sub(r'\bDEFAULT\s+FALSE\b', 'DEFAULT 0', text, flags=re.IGNORECASE)
    return text

def transform_dml(sql: str) -> str:
    text = re.sub(r'--[^\n]*', '', sql)
    # Convert Access #YYYY-MM-DD HH:MM:SS# or #YYYY-MM-DD# to 'YYYY-MM-DD...'
    text = re.sub(r'#(\d{4}-\d{2}-\d{2}(?:\s+\d{2}:\d{2}:\d{2})?)#', r"'\1'", text)
    # Convert DATEADD('h', N, NOW()) -> datetime('now', 'N hours')
    text = re.sub(r"DATEADD\('h',\s*(-?\d+),\s*NOW\(\)\)", r"datetime('now', '\1 hours')", text, flags=re.IGNORECASE)
    # Convert DATEADD('n', N, NOW()) -> datetime('now', 'N minutes')
    text = re.sub(r"DATEADD\('n',\s*(-?\d+),\s*NOW\(\)\)", r"datetime('now', '\1 minutes')", text, flags=re.IGNORECASE)
    # Convert standalone NOW() -> datetime('now')
    text = re.sub(r"\bNOW\(\)", "datetime('now')", text, flags=re.IGNORECASE)
    # Convert TRUE/FALSE to 1/0
    text = re.sub(r'\bTRUE\b', '1', text, flags=re.IGNORECASE)
    text = re.sub(r'\bFALSE\b', '0', text, flags=re.IGNORECASE)
    return text

def build_sqlite_db():
    print(f"Creating cross-platform SQLite database at: {SQLITE_DB_PATH}")
    if os.path.exists(SQLITE_DB_PATH):
        os.remove(SQLITE_DB_PATH)

    conn = sqlite3.connect(SQLITE_DB_PATH)
    cursor = conn.cursor()
    cursor.execute("PRAGMA foreign_keys = ON;")

    with open(SCHEMA_PATH, "r", encoding="utf-8") as f:
        schema_raw = f.read()

    schema_sql = transform_ddl(schema_raw)
    
    # Execute DDL statements
    statements = [stmt.strip() for stmt in schema_sql.split(";") if stmt.strip()]
    for stmt in statements:
        if not stmt:
            continue
        try:
            cursor.execute(stmt)
        except Exception as e:
            if not stmt.upper().startswith("DROP"):
                print(f"Error executing DDL statement:\n{stmt}\nError: {e}")
                raise

    print("Schema created successfully.")

    with open(DATASET_PATH, "r", encoding="utf-8") as f:
        dataset_raw = f.read()

    dataset_sql = transform_dml(dataset_raw)
    dml_statements = [stmt.strip() for stmt in dataset_sql.split(";") if stmt.strip()]
    
    inserted = 0
    for stmt in dml_statements:
        clean = stmt.strip()
        if not clean:
            continue
        try:
            cursor.execute(stmt)
            inserted += 1
        except Exception as e:
            print(f"Error executing DML statement:\n{stmt}\nError: {e}")
            raise

    conn.commit()
    print(f"Dataset loaded: {inserted} insert batches executed.")

    # Verification counts
    tables = [
        "Users", "Wards", "Assets", "Vendors", "VendorSLAs",
        "BreakdownLogs", "MaintenanceJobs", "SpareParts",
        "JobSpareParts", "RepairCosts", "EscalationLogs"
    ]
    print("\n--- Verification Record Counts ---")
    for tbl in tables:
        cursor.execute(f"SELECT COUNT(*) FROM {tbl}")
        cnt = cursor.fetchone()[0]
        print(f"  {tbl:20}: {cnt} rows")

    conn.close()
    print(f"\nDatabase successfully generated: {SQLITE_DB_PATH} ({os.path.getsize(SQLITE_DB_PATH)} bytes)")

if __name__ == "__main__":
    build_sqlite_db()
