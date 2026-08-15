import { Routes, Route } from 'react-router-dom';
import Layout from './components/Layout';
import CampaignsPage from './pages/CampaignsPage';
import CampaignPage from './pages/CampaignPage';

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<CampaignsPage />} />
        <Route path="/campaign/:name" element={<CampaignPage />} />
      </Routes>
    </Layout>
  );
}
