(async function(){
  const fallback={generatedAt:'Synthetic demonstration snapshot',departments:[
    {department:'Critical Care',assets:2,operational:2,openBreakdowns:0},
    {department:'Emergency Medicine',assets:4,operational:3,openBreakdowns:1},
    {department:'Imaging',assets:3,operational:3,openBreakdowns:0},
    {department:'Surgical Services',assets:5,operational:4,openBreakdowns:1},
    {department:'Inpatient Services',assets:6,operational:5,openBreakdowns:1}
  ]};
  let data=fallback;
  try{const response=await fetch('data/uptime.json',{cache:'no-store'});if(response.ok)data=await response.json();}catch(_){/* file:// fallback */}
  const rows=data.departments||[];const total=rows.reduce((n,r)=>n+Number(r.assets||0),0);const operational=rows.reduce((n,r)=>n+Number(r.operational||0),0);const open=rows.reduce((n,r)=>n+Number(r.openBreakdowns||0),0);const pct=total?100*operational/total:0;
  const set=(id,value)=>{const e=document.getElementById(id);if(e)e.textContent=value;};
  set('asset-total',total);set('asset-operational',operational);set('uptime-average',total?pct.toFixed(1)+'%':'N/A');set('open-breakdowns',open);set('snapshot-note','Snapshot generated: '+(data.generatedAt||'not specified')+'. Values are illustrative unless replaced by an authorised operational export.');
  const list=document.getElementById('uptime-list');if(list)list.innerHTML=rows.map(r=>{const p=r.assets?100*r.operational/r.assets:0;return `<div class="bar-row"><div class="bar-label"><span>${escapeHtml(r.department)}</span><b>${p.toFixed(1)}%</b></div><div class="bar"><i style="width:${p}%"></i></div></div>`}).join('');
  const table=document.getElementById('uptime-table');if(table)table.innerHTML=rows.map(r=>{const unavailable=Number(r.assets)-Number(r.operational);const p=r.assets?100*r.operational/r.assets:0;return `<tr><td>${escapeHtml(r.department)}</td><td>${r.assets}</td><td>${r.operational}</td><td>${unavailable}</td><td>${p.toFixed(1)}%</td></tr>`}).join('');
  function escapeHtml(v){return String(v).replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));}
})();
