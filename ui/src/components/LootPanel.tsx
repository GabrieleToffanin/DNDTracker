import { useState } from 'react';
import type { LootResource } from '../api/types';
import { addLoot } from '../api/api';

interface Props { campaignName: string; loot: LootResource[]; onRefresh: () => void; }

export default function LootPanel({ campaignName, loot, onRefresh }: Props) {
  const [show, setShow] = useState(false);

  return (
    <div>
      <div className="panel-header">
        <h3>💎 Loot ({loot.length})</h3>
        <button className="btn btn-sm btn-primary" onClick={() => setShow(true)}>+ Add Loot</button>
      </div>
      {loot.length === 0 && <p className="text-dim">No loot yet.</p>}
      <div style={{ display: 'flex', flexDirection: 'column', gap: '.35rem' }}>
        {loot.map(l => (
          <div key={l.id} className="panel flex justify-between items-center">
            <div>
              <span style={{ fontFamily: 'var(--font-heading)', color: 'var(--clr-gold-lt)' }}>{l.name}</span>
              {l.notes && <span className="text-dim" style={{ marginLeft: '.5rem', fontSize: '.85rem' }}>{l.notes}</span>}
            </div>
            {l.isMagicItem && <span className="badge badge-gold">✨ Magic</span>}
          </div>
        ))}
      </div>
      {show && <AddLootModal campaignName={campaignName} onClose={() => setShow(false)} onCreated={() => { setShow(false); onRefresh(); }} />}
    </div>
  );
}

function AddLootModal({ campaignName, onClose, onCreated }: { campaignName: string; onClose: () => void; onCreated: () => void }) {
  const [f, setF] = useState({ name: '', isMagicItem: false, notes: '' });
  const [err, setErr] = useState('');
  const [saving, setSaving] = useState(false);

  const submit = async () => {
    if (!f.name.trim()) { setErr('Name required.'); return; }
    setSaving(true);
    try { await addLoot(campaignName, f); onCreated(); }
    catch (e: any) { setErr(e?.response?.data?.detail ?? 'Failed.'); }
    finally { setSaving(false); }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>💎 Add Loot</h2>
        <div className="form-row"><label>Name *</label><input value={f.name} onChange={e => setF(p => ({ ...p, name: e.target.value }))} /></div>
        <div className="form-row"><label>Notes</label><textarea rows={2} value={f.notes} onChange={e => setF(p => ({ ...p, notes: e.target.value }))} /></div>
        <div className="form-row flex items-center gap-sm">
          <input type="checkbox" id="magic" checked={f.isMagicItem} onChange={e => setF(p => ({ ...p, isMagicItem: e.target.checked }))} style={{ width: 'auto' }} />
          <label htmlFor="magic" style={{ marginBottom: 0 }}>Magic Item</label>
        </div>
        {err && <p className="error-msg">{err}</p>}
        <div className="modal-actions">
          <button className="btn" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={submit} disabled={saving}>{saving ? 'Saving…' : 'Add'}</button>
        </div>
      </div>
    </div>
  );
}
