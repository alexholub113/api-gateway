import { BarChart3, Shield } from 'lucide-react';
import './App.css';
import { DemoRequestPanel } from './components/DemoRequestPanel';
import { ServiceHealthMonitor } from './components/ServiceHealthMonitor';

function App() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 to-gray-800 text-white">
      {/* Header */}
      <header className="bg-black bg-opacity-30 border-b border-gray-600">
        <div className="max-w-7xl mx-auto px-4 py-6">
          <div className="text-center">
            <h1 className="text-4xl font-bold mb-2 bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
              🚀 API Gateway Control Center
            </h1>
            <p className="text-xl text-gray-300">
              Monitor and control your API Gateway in real-time
            </p>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 py-8">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Demo Request Simulator */}
          <div className="lg:col-span-2">
            <DemoRequestPanel />
          </div>

          {/* Service Health Monitor */}
          <div className="lg:col-span-2">
            <ServiceHealthMonitor />
          </div>

          {/* Architecture Visualization */}
          <div className="bg-gray-800 rounded-xl p-6 border border-gray-600">
            <div className="text-center mb-6">
              <h3 className="text-2xl font-bold text-white mb-2 flex items-center justify-center gap-2">
                <Shield className="w-6 h-6 text-primary" />
                Gateway Architecture
              </h3>
              <p className="text-gray-300">Visual representation of the API Gateway flow</p>
            </div>

            <div className="flex flex-col items-center space-y-4 py-8">
              <div className="bg-primary text-white px-6 py-3 rounded-lg font-semibold min-w-32 text-center">
                📱 Client
              </div>
              <div className="text-2xl text-gray-400">↓</div>
              <div className="bg-success text-white px-6 py-3 rounded-lg font-semibold min-w-32 text-center">
                🚪 API Gateway
              </div>
              <div className="text-2xl text-gray-400">↓</div>
              <div className="flex flex-col md:flex-row gap-4 items-center">
                <div className="bg-warning text-white px-4 py-2 rounded-lg font-medium text-center">
                  ⚡ Rate Limiting
                </div>
                <div className="bg-primary text-white px-4 py-2 rounded-lg font-medium text-center">
                  💾 Caching
                </div>
                <div className="bg-success text-white px-4 py-2 rounded-lg font-medium text-center">
                  ⚖️ Load Balancer
                </div>
              </div>
              <div className="text-2xl text-gray-400">↓</div>
              <div className="bg-danger text-white px-6 py-3 rounded-lg font-semibold min-w-32 text-center">
                🎯 Target Services
              </div>
            </div>
          </div>

          {/* Metrics Overview */}
          <div className="bg-gray-800 rounded-xl p-6 border border-gray-600">
            <div className="text-center mb-6">
              <h3 className="text-2xl font-bold text-white mb-2 flex items-center justify-center gap-2">
                <BarChart3 className="w-6 h-6 text-primary" />
                Gateway Metrics
              </h3>
              <p className="text-gray-300">Real-time performance indicators</p>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="bg-darker p-4 rounded-lg text-center border border-gray-600">
                <div className="text-2xl font-bold text-primary mb-1">1,234</div>
                <div className="text-sm text-gray-400 uppercase tracking-wide">Requests/min</div>
                <div className="text-xs text-success mt-1">↗️ +12%</div>
              </div>
              <div className="bg-darker p-4 rounded-lg text-center border border-gray-600">
                <div className="text-2xl font-bold text-success mb-1">45ms</div>
                <div className="text-sm text-gray-400 uppercase tracking-wide">Avg Response</div>
                <div className="text-xs text-success mt-1">↗️ -5ms</div>
              </div>
              <div className="bg-darker p-4 rounded-lg text-center border border-gray-600">
                <div className="text-2xl font-bold text-warning mb-1">85%</div>
                <div className="text-sm text-gray-400 uppercase tracking-wide">Cache Hit Rate</div>
                <div className="text-xs text-success mt-1">↗️ +3%</div>
              </div>
              <div className="bg-darker p-4 rounded-lg text-center border border-gray-600">
                <div className="text-2xl font-bold text-danger mb-1">12</div>
                <div className="text-sm text-gray-400 uppercase tracking-wide">Rate Limited</div>
                <div className="text-xs text-gray-400 mt-1">-</div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}

export default App;
