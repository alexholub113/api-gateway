import './App.css';
import { DemoRequestPanel } from './components/DemoRequestPanel';
import { GatewayArchitecture } from './components/GatewayArchitecture';
import { GatewayMetrics } from './components/GatewayMetrics';
import { ServiceHealthMonitor } from './components/ServiceHealthMonitor';

function App() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 to-gray-800 text-white">
      {/* Header */}
      <header className="bg-black bg-opacity-30 border-b border-gray-600">
        <div className="max-w-7xl mx-auto px-4 py-4">
          <div className="text-center">
            <h1 className="text-3xl font-bold mb-1 bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
              🚀 API Gateway Control Center
            </h1>
            <p className="text-lg text-gray-300">
              Monitor and control your API Gateway in real-time
            </p>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 py-4">
        <div className="space-y-8">
          {/* Demo Request Simulator */}
          <div>
            <DemoRequestPanel />
          </div>

          {/* Vertical Separator */}
          <div className="flex justify-center">
            <div className="w-px h-8 bg-gradient-to-b from-transparent via-gray-600 to-transparent"></div>
          </div>

          {/* Service Health Monitor */}
          <div>
            <ServiceHealthMonitor />
          </div>

          {/* Vertical Separator */}
          <div className="flex justify-center">
            <div className="w-px h-8 bg-gradient-to-b from-transparent via-gray-600 to-transparent"></div>
          </div>

          {/* Gateway Metrics */}
          <div>
            <GatewayMetrics />
          </div>

          {/* Vertical Separator */}
          <div className="flex justify-center">
            <div className="w-px h-8 bg-gradient-to-b from-transparent via-gray-600 to-transparent"></div>
          </div>

          {/* Gateway Architecture */}
          <div>
            <GatewayArchitecture />
          </div>
        </div>
      </main>
    </div>
  );
}

export default App;
