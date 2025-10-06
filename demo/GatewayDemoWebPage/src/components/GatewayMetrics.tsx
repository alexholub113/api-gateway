import { Activity, AlertTriangle, BarChart3, Clock, Database, Shield, Users, Zap } from 'lucide-react';

export function GatewayMetrics() {
  return (
    <div className="bg-gray-800 rounded-xl p-8 border border-gray-600">
      <div className="text-center mb-8">
        <h3 className="text-3xl font-bold text-white mb-3 flex items-center justify-center gap-3">
          <BarChart3 className="w-8 h-8 text-blue-500" />
          Gateway Metrics Dashboard
        </h3>
        <p className="text-lg text-gray-300">Real-time performance indicators and system health</p>
      </div>

      {/* Primary Metrics */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-6 mb-8">
        <div className="bg-gradient-to-br from-blue-900 to-blue-800 p-6 rounded-xl text-center border border-blue-600 shadow-lg">
          <Activity className="w-8 h-8 text-blue-400 mx-auto mb-3" />
          <div className="text-3xl font-bold text-blue-400 mb-2">1,234</div>
          <div className="text-sm text-blue-200 uppercase tracking-wide mb-2">Requests/min</div>
          <div className="text-xs text-green-400 flex items-center justify-center gap-1">
            <span>↗️ +12%</span>
            <span className="text-blue-300">vs last hour</span>
          </div>
        </div>

        <div className="bg-gradient-to-br from-green-900 to-green-800 p-6 rounded-xl text-center border border-green-600 shadow-lg">
          <Clock className="w-8 h-8 text-green-400 mx-auto mb-3" />
          <div className="text-3xl font-bold text-green-400 mb-2">45ms</div>
          <div className="text-sm text-green-200 uppercase tracking-wide mb-2">Avg Response</div>
          <div className="text-xs text-green-400 flex items-center justify-center gap-1">
            <span>↗️ -5ms</span>
            <span className="text-green-300">improvement</span>
          </div>
        </div>

        <div className="bg-gradient-to-br from-yellow-900 to-yellow-800 p-6 rounded-xl text-center border border-yellow-600 shadow-lg">
          <Database className="w-8 h-8 text-yellow-400 mx-auto mb-3" />
          <div className="text-3xl font-bold text-yellow-400 mb-2">85%</div>
          <div className="text-sm text-yellow-200 uppercase tracking-wide mb-2">Cache Hit Rate</div>
          <div className="text-xs text-green-400 flex items-center justify-center gap-1">
            <span>↗️ +3%</span>
            <span className="text-yellow-300">efficiency</span>
          </div>
        </div>

        <div className="bg-gradient-to-br from-red-900 to-red-800 p-6 rounded-xl text-center border border-red-600 shadow-lg">
          <Zap className="w-8 h-8 text-red-400 mx-auto mb-3" />
          <div className="text-3xl font-bold text-red-400 mb-2">12</div>
          <div className="text-sm text-red-200 uppercase tracking-wide mb-2">Rate Limited</div>
          <div className="text-xs text-red-300 flex items-center justify-center gap-1">
            <span>-</span>
            <span className="text-red-300">last 5min</span>
          </div>
        </div>
      </div>

      {/* Secondary Metrics */}
      <div className="grid grid-cols-2 md:grid-cols-6 gap-4 mb-8">
        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <Shield className="w-6 h-6 text-purple-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-purple-400 mb-1">2.1K</div>
          <div className="text-xs text-gray-400 uppercase">Auth Requests</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <Users className="w-6 h-6 text-cyan-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-cyan-400 mb-1">847</div>
          <div className="text-xs text-gray-400 uppercase">Active Users</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <AlertTriangle className="w-6 h-6 text-orange-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-orange-400 mb-1">3</div>
          <div className="text-xs text-gray-400 uppercase">Circuit Open</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <BarChart3 className="w-6 h-6 text-indigo-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-indigo-400 mb-1">99.7%</div>
          <div className="text-xs text-gray-400 uppercase">Uptime</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <Database className="w-6 h-6 text-emerald-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-emerald-400 mb-1">156MB</div>
          <div className="text-xs text-gray-400 uppercase">Cache Size</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <Activity className="w-6 h-6 text-pink-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-pink-400 mb-1">8</div>
          <div className="text-xs text-gray-400 uppercase">Services</div>
        </div>
      </div>

      {/* Status Summary */}
      <div className="bg-gray-900 rounded-xl p-6 border border-gray-600">
        <h4 className="text-lg font-bold text-white mb-4 text-center">System Health Summary</h4>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="text-center">
            <div className="text-2xl font-bold text-green-400 mb-1">Excellent</div>
            <div className="text-sm text-gray-400 mb-2">Overall Performance</div>
            <div className="w-full bg-gray-700 rounded-full h-2">
              <div className="bg-green-400 h-2 rounded-full" style={{ width: '94%' }}></div>
            </div>
            <div className="text-xs text-green-400 mt-1">94% optimal</div>
          </div>

          <div className="text-center">
            <div className="text-2xl font-bold text-blue-400 mb-1">Stable</div>
            <div className="text-sm text-gray-400 mb-2">Load Balancing</div>
            <div className="w-full bg-gray-700 rounded-full h-2">
              <div className="bg-blue-400 h-2 rounded-full" style={{ width: '87%' }}></div>
            </div>
            <div className="text-xs text-blue-400 mt-1">87% efficiency</div>
          </div>

          <div className="text-center">
            <div className="text-2xl font-bold text-yellow-400 mb-1">Good</div>
            <div className="text-sm text-gray-400 mb-2">Error Rate</div>
            <div className="w-full bg-gray-700 rounded-full h-2">
              <div className="bg-yellow-400 h-2 rounded-full" style={{ width: '15%' }}></div>
            </div>
            <div className="text-xs text-yellow-400 mt-1">0.3% errors</div>
          </div>
        </div>
      </div>
    </div>
  );
}