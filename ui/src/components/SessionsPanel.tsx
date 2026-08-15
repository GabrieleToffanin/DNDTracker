import { useState } from 'react';
import type { SessionLogEntry } from '../api/types';
import { addSessionLog } from '../api/api';

interface Props { campaignName: string; sessions: SessionLogEntry[]; onRefresh: () => void; }

export default function SessionsPanel({ campaignName, sessions, onRefresh }: Props) {
  const [show, setShow] = useState(false);
  const sorted = [...sessions].sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());

  return (
    <div>
      <div className="panel-header">
        <h3>📖 Session Logs ({sessions.length})</h3>
        <button className="btn btn-sm btn-primary" onClick={() => setShow(true)}>+ Add Session</button>
      </div>
      {sorted.length === 0 && <p className="text-dim">No session logs yet.</p>}
      {sorted.map(s => (
        <div key={s.id} className="panel" style={{ marginBottom: '.5rem' }}>
          <div className="flex justify-between items-center mb-sm">
            <strong style={{ fontFamily: 'var(--font-heading)', color: 'var(--clr-gold-lt)' }}>
              {new Date(s.date).toLocaleDateString(undefined, { year: 'numeric', month: 'long', day: 'numeric' })}
            </strong>
            <span className="badge badge-dim">{s.durationMinutes} min</span>
          </div>
          <p style={{ marginBottom: '.3rem' }}>{s.summary}</p>
          {s.dungeonMasterNotes && <p className="text-dim" style={{ fontSize: '.85rem', fontStyle: 'italic' }}>DM Notes: {s.dungeonMasterNotes}</p>}
        </div>
      ))}
      {show && <AddSessionModal campaignName={campaignName} onClose={() => setShow(false)} onCreated={() => { setShow(false); onRefresh(); }} />}
    </div>
  );
}

function AddSessionModal({ campaignName, onClose, onCreated }: { campaignName: string; onClose: () => void; onCreated: () => void }) {
  const today = new Date().toISOString().split('T')[0];
  const [f, setF] = useState({ date: today, durationMinutes: 180, summary: '', dungeonMasterNotes: '' });
  const [err, setErr] = useState('');
  const [saving, setSaving] = useState(false);
  const s = (k: string, v: string | number) => setF(p => ({ ...p, [k]: v }));

  const submit = async () => {
    if (!f.summary.trim()) { setErr('Summary required.'); return; }
    setSaving(true);
    try {
      await addSessionLog(campaignName, { ...f, date: new Date(f.date).toISOString() });
      onCreated();
    } catch (e: any) { setErr(e?.response?.data?.detail ?? 'Failed.'); }
    finally { setSaving(false); }
  };

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>📖 Add Session Log</h2>
        <div className="form-grid form-grid-2">
          <div className="form-row"><label>Date</label><input type="date" value={f.date} onChange={e => s('date', e.target.value)} /></div>
          <div className="form-row"><label>Duration (min)</label><input type="number" min={1} value={f.durationMinutes} onChange={e => s('durationMinutes', +e.target.value)} /></div>
        </div>
        <div className="form-row"><label>Summary *</label><textarea rows={3} value={f.summary} onChange={e => s('summary', e.target.value)} /></div>
        <div className="form-row"><label>DM Notes</label><textarea rows={2} value={f.dungeonMasterNotes} onChange={e => s('dungeonMasterNotes', e.target.value)} /></div>
        {err && <p className="error-msg">{err}</p>}
        <div className="modal-actions">
          <button className="btn" onClick={onClose}>Cancel</button>
          <button className="btn btn-primary" onClick={submit} disabled={saving}>{saving ? 'Saving…' : 'Add Session'}</button>
        </div>
      </div>
    </div>
  );
}
