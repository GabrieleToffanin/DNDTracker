import { useState } from 'react';
import type { QuestResource } from '../api/types';
import { addQuest } from '../api/api';

interface Props { campaignName: string; quests: QuestResource[]; onRefresh: () => void; }

const STATUS_COLOR: Record<string, string> = { Active: 'badge-gold', Completed: 'badge-green', Failed: 'badge-red' };

export default function QuestsPanel({ campaignName, quests, onRefresh }: Props) {
  const [show, setShow] = useState(false);

  return (
    <div>
      <div className="panel-header">
        <h3>📜 Quests ({quests.length})</h3>
        <button className="btn btn-sm btn-primary" onClick={() => setShow(true)}>+ Add Quest</button>
      </div>
      {quests.length === 0 && <p className="text-dim">No quests yet.</p>}
      {quests.map(q => (
        <div key={q.id} className="panel" style={{ marginBottom: '.5rem' }}>
          <div className="flex justify-between items-center mb-sm">
            <strong style={{ fontFamily: 'var(--font-heading)', color: 'var(--clr-gold-lt)' }}>{q.title}</strong>
            <span className={`badge ${STATUS_COLOR[q.status] ?? 'badge-dim'}`}>{q.status}</span>
          </div>
          {q.description && <p className="text-dim" style={{ fontSize: '.9rem' }}>{q.description}</p>}
        </div>
      ))}
      {show && (
        <AddQuestModal campaignName={campaignName} onClose={() => setShow(false)} onCreated={() => { setShow(false); onRefresh(); }} />
      )}
    </div>
  );
}

function AddQuestModal({ campaignName, onClose, onCreated }: { campaignName: string; onClose: () => void; onCreated: () => void }) {
  const [f, setF] = useState({ title: '', status: 'Active', description: '' });
  const [err, setErr] = useState('');
  const [saving, setSaving] = useState(false);
  const s = (k: string, v: string) => setF(p => ({ ...p, [k]: v }));

  const submit = async () => {
    if (!f.title.trim()) { setErr('Title required.'); return; }
    setSaving(true);
    try { await addQuest(campaignName, f); onCreated(); }
    catch (e: any) { setErr(e?.response?.data?.detail ?? 'Failed.'); }
    finally { setSaving(false); }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>📜 Add Quest</h2>
        <div className="form-row"><label>Title *</label><input value={f.title} onChange={e => s('title', e.target.value)} /></div>
        <div className="form-row"><label>Status</label>
          <select value={f.status} onChange={e => s('status', e.target.value)}>
            <option>Active</option><option>Completed</option><option>Failed</option>
          </select>
        </div>
        <div className="form-row"><label>Description</label><textarea rows={3} value={f.description} onChange={e => s('description', e.target.value)} /></div>
        {err && <p className="error-msg">{err}</p>}
        <div className="modal-actions">
          <button className="btn" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={submit} disabled={saving}>{saving ? 'Saving…' : 'Add Quest'}</button>
        </div>
      </div>
    </div>
  );
}
