Imports System
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports HBMS.Desktop.Services

Namespace HBMS.Desktop.Forms
    Public Class ReferenceDataForm
        Inherits Form
        Public Sub New()
            Text = "HBMS | Manage reference data" : StartPosition = FormStartPosition.CenterParent : ClientSize = New Size(650, 470) : MinimumSize = New Size(650, 470)
            Dim tabs As New TabControl() With {.Dock = DockStyle.Fill}
            tabs.TabPages.Add(BuildUserTab()) : tabs.TabPages.Add(BuildVendorTab()) : tabs.TabPages.Add(BuildAssetTab()) : tabs.TabPages.Add(BuildPartTab()) : tabs.TabPages.Add(BuildSlaTab())
            Controls.Add(tabs)
        End Sub
        Private Function NewTab(caption As String) As TabPage
            Return New TabPage(caption)
        End Function
        Private Sub AddInput(page As TabPage, label As String, control As Control, y As Integer)
            page.Controls.Add(New Label() With {.Text = label, .Location = New Point(25, y + 4), .AutoSize = True})
            control.Location = New Point(190, y) : control.Width = 360 : page.Controls.Add(control)
        End Sub
        Private Function AddSave(page As TabPage, caption As String, action As Action) As Button
            Dim b As New Button() With {.Text = caption, .Location = New Point(190, 365), .Width = 140, .Height = 32}
            AddHandler b.Click, Sub()
                                    Try
                                        action() : MessageBox.Show("Record saved.", "HBMS", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                    Catch ex As Exception
                                        MessageBox.Show(ex.Message, "Unable to save record", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    End Try
                                End Sub
            page.Controls.Add(b) : Return b
        End Function
        Private Function BuildUserTab() As TabPage
            Dim p = NewTab("Staff accounts"), n As New TextBox(), u As New TextBox(), pw As New TextBox() With {.UseSystemPasswordChar = True}, role As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}
            role.Items.AddRange({"ADMIN", "SUPERVISOR", "ENGINEER", "WARD_REPORTER", "STORE", "EXECUTIVE"}) : role.SelectedIndex = 2
            AddInput(p, "Full name", n, 35) : AddInput(p, "Username", u, 90) : AddInput(p, "Temporary password (10+ chars)", pw, 145) : AddInput(p, "Role", role, 200)
            AddSave(p, "Create account", Sub() HbmsService.AddUser(n.Text, u.Text, pw.Text, CStr(role.SelectedItem))) : Return p
        End Function
        Private Function BuildVendorTab() As TabPage
            Dim p = NewTab("Vendors"), code As New TextBox(), name As New TextBox(), contact As New TextBox(), phone As New TextBox(), email As New TextBox()
            AddInput(p, "Vendor code", code, 35) : AddInput(p, "Vendor name", name, 90) : AddInput(p, "Contact person", contact, 145) : AddInput(p, "Phone", phone, 200) : AddInput(p, "Email", email, 255)
            AddSave(p, "Add vendor", Sub() HbmsService.AddVendor(code.Text, name.Text, contact.Text, phone.Text, email.Text)) : Return p
        End Function
        Private Function BuildAssetTab() As TabPage
            Dim p = NewTab("Equipment assets"), code As New TextBox(), name As New TextBox(), category As New TextBox(), ward As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}, crit As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}, vendor As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}
            ward.DataSource = HbmsService.GetWards() : ward.DisplayMember = "WardName" : ward.ValueMember = "WardID"
            crit.Items.AddRange({"CRITICAL_CARE", "HIGH", "STANDARD"}) : crit.SelectedIndex = 2
            Dim vendors = Database.Db.Query("SELECT VendorID,VendorName FROM Vendors WHERE IsActive=True ORDER BY VendorName")
            Dim vendorTable As DataTable = vendors.Copy() : Dim none = vendorTable.NewRow() : none("VendorID") = 0 : none("VendorName") = "No assigned vendor" : vendorTable.Rows.InsertAt(none, 0)
            vendor.DataSource = vendorTable : vendor.DisplayMember = "VendorName" : vendor.ValueMember = "VendorID"
            AddInput(p, "Asset code", code, 25) : AddInput(p, "Equipment name", name, 75) : AddInput(p, "Category", category, 125) : AddInput(p, "Ward", ward, 175) : AddInput(p, "Criticality", crit, 225) : AddInput(p, "Service vendor", vendor, 275)
            AddSave(p, "Register asset", Sub() HbmsService.AddAsset(code.Text, name.Text, category.Text, CInt(ward.SelectedValue), CStr(crit.SelectedItem), If(CInt(vendor.SelectedValue) = 0, DBNull.Value, CObj(CInt(vendor.SelectedValue))))) : Return p
        End Function
        Private Function BuildPartTab() As TabPage
            Dim p = NewTab("Spare parts"), code As New TextBox(), name As New TextBox(), qty As New NumericUpDown() With {.Maximum = 100000}, reorder As New NumericUpDown() With {.Maximum = 100000}, cost As New NumericUpDown() With {.Maximum = 100000000, .DecimalPlaces = 2}
            AddInput(p, "Part code", code, 45) : AddInput(p, "Part name", name, 100) : AddInput(p, "Opening quantity", qty, 155) : AddInput(p, "Reorder level", reorder, 210) : AddInput(p, "Unit cost", cost, 265)
            AddSave(p, "Add spare part", Sub() HbmsService.AddPart(code.Text, name.Text, CInt(qty.Value), CInt(reorder.Value), cost.Value)) : Return p
        End Function
        Private Function BuildSlaTab() As TabPage
            Dim p = NewTab("Vendor SLAs"), vendor As New ComboBox() With {.DropDownStyle = ComboBoxStyle.DropDownList}, name As New TextBox() With {.Text = "Standard SLA"}, response As New NumericUpDown() With {.Maximum = 720, .Minimum = 0.25D, .DecimalPlaces = 2, .Value = 4}, resolution As New NumericUpDown() With {.Maximum = 2000, .Minimum = 0.5D, .DecimalPlaces = 2, .Value = 24}
            vendor.DataSource = Database.Db.Query("SELECT VendorID,VendorName FROM Vendors WHERE IsActive=True ORDER BY VendorName") : vendor.DisplayMember = "VendorName" : vendor.ValueMember = "VendorID"
            AddInput(p, "Vendor", vendor, 55) : AddInput(p, "SLA name", name, 110) : AddInput(p, "Response hours", response, 165) : AddInput(p, "Resolution hours", resolution, 220)
            AddSave(p, "Add SLA", Sub() HbmsService.AddVendorSla(CInt(vendor.SelectedValue), name.Text, CDbl(response.Value), CDbl(resolution.Value))) : Return p
        End Function
    End Class
End Namespace
