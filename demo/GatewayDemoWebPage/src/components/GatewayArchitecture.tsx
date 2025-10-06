import { Activity, AlertTriangle, BarChart3, Database, Globe, Lock, RefreshCw, Server, Shield, Zap } from 'lucide-react';

export function GatewayArchitecture() {
  return (
    <div className="bg-gray-800 rounded-xl p-4 border border-gray-600">
      <div className="text-center mb-4">
        <h3 className="text-2xl font-bold text-white mb-2 flex items-center justify-center gap-2">
          <Shield className="w-6 h-6 text-blue-500" />
          API Gateway Architecture
        </h3>
        <p className="text-gray-300">Complete request flow and processing pipeline</p>
      </div>

      <div className="space-y-4">
        {/* Client Layer */}
        <div className="flex justify-center">
          <div className="bg-gradient-to-r from-purple-600 to-purple-700 text-white px-4 py-2 rounded-lg font-semibold shadow-lg min-w-48 text-center border border-purple-500">
            <Globe className="w-4 h-4 inline-block mr-2" />
            Client Applications
          </div>
        </div>

        {/* Flow Arrow */}
        <div className="flex justify-center">
          <div className="text-2xl text-blue-400">↓</div>
        </div>

        {/* API Gateway Core */}
        <div className="bg-gradient-to-r from-blue-600 to-blue-700 rounded-lg p-3 border border-blue-500">
          <h4 className="text-lg font-bold text-white text-center mb-3 flex items-center justify-center gap-2">
            <Shield className="w-5 h-5" />
            API Gateway Core
          </h4>
          
          {/* Request Pipeline */}
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-2 mb-3">
            <div className="bg-black bg-opacity-30 rounded-lg p-2 text-center border border-blue-400">
              <Activity className="w-5 h-5 text-green-400 mx-auto mb-1" />
              <div className="text-white font-medium text-sm">Request Ingress</div>
              <div className="text-blue-200 text-xs">HTTP/HTTPS</div>
            </div>
            <div className="bg-black bg-opacity-30 rounded-lg p-2 text-center border border-blue-400">
              <BarChart3 className="w-5 h-5 text-yellow-400 mx-auto mb-1" />
              <div className="text-white font-medium text-sm">Route Resolution</div>
              <div className="text-blue-200 text-xs">Path Matching</div>
            </div>
            <div className="bg-black bg-opacity-30 rounded-lg p-2 text-center border border-blue-400">
              <RefreshCw className="w-5 h-5 text-purple-400 mx-auto mb-1" />
              <div className="text-white font-medium text-sm">Pipeline Execute</div>
              <div className="text-blue-200 text-xs">Module Chain</div>
            </div>
          </div>
        </div>

        {/* Flow Arrow */}
        <div className="flex justify-center">
          <div className="text-2xl text-green-400">↓</div>
        </div>

        {/* Processing Modules */}
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-3">
          {/* Rate Limiting */}
          <div className="bg-gradient-to-br from-orange-600 to-red-600 rounded-lg p-3 border border-orange-500 transform hover:scale-105 transition-transform">
            <div className="text-center">
              <Zap className="w-6 h-6 text-yellow-300 mx-auto mb-2" />
              <h5 className="text-sm font-bold text-white mb-1">Rate Limiting</h5>
              <div className="space-y-1 text-xs text-orange-100">
                <div>• Throttling</div>
                <div>• Token bucket</div>
              </div>
            </div>
          </div>

          {/* Caching */}
          <div className="bg-gradient-to-br from-green-600 to-emerald-600 rounded-lg p-3 border border-green-500 transform hover:scale-105 transition-transform">
            <div className="text-center">
              <Database className="w-6 h-6 text-green-300 mx-auto mb-2" />
              <h5 className="text-sm font-bold text-white mb-1">Caching</h5>
              <div className="space-y-1 text-xs text-green-100">
                <div>• In-memory</div>
                <div>• TTL mgmt</div>
              </div>
            </div>
          </div>

          {/* Authentication */}
          <div className="bg-gradient-to-br from-purple-600 to-violet-600 rounded-lg p-3 border border-purple-500 transform hover:scale-105 transition-transform">
            <div className="text-center">
              <Lock className="w-6 h-6 text-purple-300 mx-auto mb-2" />
              <h5 className="text-sm font-bold text-white mb-1">AuthZN</h5>
              <div className="space-y-1 text-xs text-purple-100">
                <div>• JWT validation</div>
                <div>• Policy enforce</div>
              </div>
            </div>
          </div>

          {/* Load Balancing */}
          <div className="bg-gradient-to-br from-blue-600 to-cyan-600 rounded-lg p-3 border border-blue-500 transform hover:scale-105 transition-transform">
            <div className="text-center">
              <Activity className="w-6 h-6 text-blue-300 mx-auto mb-2" />
              <h5 className="text-sm font-bold text-white mb-1">Load Balancer</h5>
              <div className="space-y-1 text-xs text-blue-100">
                <div>• Round robin</div>
                <div>• Health check</div>
              </div>
            </div>
          </div>

          {/* Circuit Breaker */}
          <div className="bg-gradient-to-br from-yellow-600 to-orange-600 rounded-lg p-3 border border-yellow-500 transform hover:scale-105 transition-transform">
            <div className="text-center">
              <AlertTriangle className="w-6 h-6 text-yellow-300 mx-auto mb-2" />
              <h5 className="text-sm font-bold text-white mb-1">Circuit Breaker</h5>
              <div className="space-y-1 text-xs text-yellow-100">
                <div>• Failure detect</div>
                <div>• Fallback</div>
              </div>
            </div>
          </div>

          {/* Metrics & Logging */}
          <div className="bg-gradient-to-br from-indigo-600 to-purple-600 rounded-lg p-3 border border-indigo-500 transform hover:scale-105 transition-transform">
            <div className="text-center">
              <BarChart3 className="w-6 h-6 text-indigo-300 mx-auto mb-2" />
              <h5 className="text-sm font-bold text-white mb-1">Observability</h5>
              <div className="space-y-1 text-xs text-indigo-100">
                <div>• Metrics</div>
                <div>• Tracing</div>
              </div>
            </div>
          </div>
        </div>

        {/* Flow Arrow */}
        <div className="flex justify-center">
          <div className="text-2xl text-purple-400">↓</div>
        </div>

        {/* Target Services */}
        <div className="bg-gradient-to-r from-gray-700 to-gray-800 rounded-lg p-3 border border-gray-500">
          <h4 className="text-lg font-bold text-white text-center mb-3 flex items-center justify-center gap-2">
            <Server className="w-5 h-5 text-gray-300" />
            Target Services
          </h4>
          
          <div className="grid grid-cols-3 gap-2">
            <div className="bg-black bg-opacity-30 rounded-lg p-2 text-center border border-gray-400">
              <Server className="w-5 h-5 text-blue-400 mx-auto mb-1" />
              <div className="text-white font-medium text-sm">Service A</div>
              <div className="text-gray-300 text-xs">3 instances</div>
              <div className="text-green-400 text-xs">● Healthy</div>
            </div>
            <div className="bg-black bg-opacity-30 rounded-lg p-2 text-center border border-gray-400">
              <Server className="w-5 h-5 text-green-400 mx-auto mb-1" />
              <div className="text-white font-medium text-sm">Service B</div>
              <div className="text-gray-300 text-xs">2 instances</div>
              <div className="text-green-400 text-xs">● Healthy</div>
            </div>
            <div className="bg-black bg-opacity-30 rounded-lg p-2 text-center border border-gray-400">
              <Server className="w-5 h-5 text-yellow-400 mx-auto mb-1" />
              <div className="text-white font-medium text-sm">Service C</div>
              <div className="text-gray-300 text-xs">1 instance</div>
              <div className="text-yellow-400 text-xs">● Degraded</div>
            </div>
          </div>
        </div>

        {/* Request Flow Indicators */}
        <div className="bg-gray-900 rounded-lg p-3 border border-gray-600">
          <h4 className="text-lg font-bold text-white text-center mb-3">Request Flow Types</h4>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {/* Normal Flow */}
            <div className="space-y-2">
              <h5 className="text-sm font-semibold text-green-400">✅ Normal Flow</h5>
              <div className="text-xs text-gray-300 space-y-1">
                <div>1. Client → API Gateway</div>
                <div>2. Rate Limit → ✅ Allow</div>
                <div>3. Cache → ❌ Miss</div>
                <div>4. Load Balancer → Select</div>
                <div>5. Proxy → Target Service</div>
                <div>6. Response → Cache & Return</div>
              </div>
            </div>

            {/* AuthZN Flow */}
            <div className="space-y-2">
              <h5 className="text-sm font-semibold text-purple-400">🔐 AuthZN Flow</h5>
              <div className="text-xs text-gray-300 space-y-1">
                <div>1. Client → API Gateway</div>
                <div>2. Rate Limit → ✅ Allow</div>
                <div>3. AuthZN → Validate</div>
                <div>4. Authorization → ✅ Grant</div>
                <div>5. Proxy → Target Service</div>
                <div>6. Response → Return</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}