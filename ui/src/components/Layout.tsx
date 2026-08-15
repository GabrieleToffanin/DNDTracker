import type { ReactNode } from 'react';
import { Link } from 'react-router-dom';
import styles from './Layout.module.css';

export default function Layout({ children }: { children: ReactNode }) {
  return (
    <div className={styles.root}>
      <header className={styles.header}>
        <Link to="/" className={styles.brand}>
          <span className={styles.brandIcon}>⚔</span>
          <span className={styles.brandText}>DNDTracker</span>
        </Link>
        <nav className={styles.nav}>
          <Link to="/">Campaigns</Link>
        </nav>
      </header>
      <main className={styles.main}>{children}</main>
    </div>
  );
}
