import { useState } from 'react';
import type { DiceRollResult } from '../api/types';
import { rollDice } from '../api/api';
import styles from './DicePanel.module.css';

const QUICK_DICE = ['1d4', '1d6', '1d8', '1d10', '1d12', '1d20', '1d100', '2d6', '4d6'];

interface Props { campaignName: string; }

export default function DicePanel({ campaignName }: Props) {
  const [expression, setExpression] = useState('1d20');
  const [modifier, setModifier] = useState(0);
  const [context, setContext] = useState('');
  const [result, setResult] = useState<DiceRollResult | null>(null);
  const [rolling, setRolling] = useState(false);
  const [err, setErr] = useState('');
  const [history, setHistory] = useState<DiceRollResult[]>([]);

  const roll = async () => {
    if (!expression.trim()) return;
    setRolling(true);
    setErr('');
    try {
      const r = await rollDice(campaignName, { expression, modifier, context: context || undefined });
      setResult(r);
      setHistory(prev => [r, ...prev].slice(0, 20));
    } catch (e: any) {
      setErr(e?.response?.data?.detail ?? 'Failed to roll.');
    } finally { setRolling(false); }
  };

  const handleKey = (e: React.KeyboardEvent) => { if (e.key === 'Enter') roll(); };

  return (
    <div className={styles.container}>
      <div className={styles.roller}>
        <h3 className="mb-md">🎲 Dice Roller</h3>

        <div className={styles.quickDice}>
          {QUICK_DICE.map(d => (
            <button key={d} className={`btn btn-sm ${expression === d ? 'btn-primary' : ''}`} onClick={() => setExpression(d)}>{d}</button>
          ))}
        </div>

        <div className="form-grid form-grid-2 mt-sm">
          <div className="form-row">
            <label>Expression</label>
            <input value={expression} onChange={e => setExpression(e.target.value)} onKeyDown={handleKey} placeholder="e.g. 2d6+3" />
          </div>
          <div className="form-row">
            <label>Modifier</label>
            <input type="number" value={modifier} onChange={e => setModifier(+e.target.value)} />
          </div>
        </div>
        <div className="form-row">
          <label>Context (optional)</label>
          <input value={context} onChange={e => setContext(e.target.value)} onKeyDown={handleKey} placeholder="e.g. Attack roll, Stealth check…" />
        </div>

        <button className="btn btn-primary w-full" onClick={roll} disabled={rolling}>{rolling ? 'Rolling…' : '🎲 Roll!'}</button>
        {err && <p className="error-msg">{err}</p>}

        {result && (
          <div className={styles.resultCard}>
            <div className={styles.resultTotal}>{result.total}</div>
            <div className="text-dim" style={{ fontSize: '.85rem' }}>
              [{result.rolls.join(', ')}]{result.modifier !== 0 ? ` + ${result.modifier}` : ''}
              {result.context ? ` — ${result.context}` : ''}
            </div>
            <div className={styles.resultExpr}>{result.expression}</div>
          </div>
        )}
      </div>

      {history.length > 0 && (
        <div className={styles.history}>
          <h3 className="mb-md">📋 History</h3>
          {history.map((r, i) => (
            <div key={i} className={styles.histRow}>
              <span className={styles.histTotal}>{r.total}</span>
              <span className={styles.histExpr}>{r.expression}{r.modifier !== 0 ? ` +${r.modifier}` : ''}</span>
              {r.context && <span className="text-dim" style={{ fontSize: '.8rem' }}>{r.context}</span>}
              <span className="text-dim" style={{ fontSize: '.78rem' }}>[{r.rolls.join(', ')}]</span>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
