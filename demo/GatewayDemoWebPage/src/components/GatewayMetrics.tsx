import axios from 'axios';
import { Activity, AlertTriangle, BarChart3, Clock, Database, RefreshCw, Shield, Users, Zap } from 'lucide-react';
import { useEffect, useState } from 'react';

interface GatewayMetricsData {
  requestsPerMinute: number;
  averageResponseTimeMs: number;
  cacheHitRatePercentage: number;
  rateLimitedRequests: number;
  authRequests: number;
  activeUsers: number;
  circuitBreakersOpen: number;
  uptimePercentage: number;
  cacheSizeMB: number;
  activeServices: number;
  overallPerformanceScore: number;
  loadBalancingEfficiency: number;
  errorRatePercentage: number;
  requestsTrend: TrendIndicator;
  responseTimeTrend: TrendIndicator;
  cacheHitTrend: TrendIndicator;
  timestamp: string;
}

interface TrendIndicator {
  percentageChange: number;
  direction: 'up' | 'down' | 'stable';
}

export function GatewayMetrics() {
  const [metrics, setMetrics] = useState<GatewayMetricsData | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [lastUpdated, setLastUpdated] = useState<Date | null>(null);

  const fetchMetrics = async () => {
    try {
      setError(null);
      const response = await axios.get<GatewayMetricsData>('https://localhost:7214/api/metrics');
      setMetrics(response.data);
      setLastUpdated(new Date());
      setIsLoading(false);
    } catch (err) {
      console.error('Error fetching metrics:', err);
      setError('Failed to fetch metrics');
      setIsLoading(false);
    }
  };

  useEffect(() => {
    // Initial fetch
    fetchMetrics();

    // Set up auto-refresh every 5 seconds
    const interval = setInterval(fetchMetrics, 5000);

    return () => clearInterval(interval);
  }, []);

  const formatTrend = (trend: TrendIndicator, isInverse = false) => {
    const isPositive = isInverse ? trend.direction === 'down' : trend.direction === 'up';
    const arrow = trend.direction === 'up' ? '↗️' : trend.direction === 'down' ? '↘️' : '→';
    const colorClass = isPositive ? 'text-green-400' : trend.direction === 'stable' ? 'text-gray-400' : 'text-red-400';
    
    return {
      arrow,
      colorClass,
      text: trend.direction === 'stable' ? 'stable' : `${trend.percentageChange > 0 ? '+' : ''}${trend.percentageChange}%`
    };
  };

  if (isLoading) {
    return (
      <div className="bg-gray-800 rounded-xl p-8 border border-gray-600">
        <div className="text-center">
          <RefreshCw className="w-8 h-8 text-blue-500 mx-auto mb-3 animate-spin" />
          <h3 className="text-2xl font-bold text-white mb-2">Loading Metrics...</h3>
          <p className="text-gray-300">Fetching real-time gateway performance data</p>
        </div>
      </div>
    );
  }

  if (error || !metrics) {
    return (
      <div className="bg-gray-800 rounded-xl p-8 border border-gray-600">
        <div className="text-center">
          <AlertTriangle className="w-8 h-8 text-red-500 mx-auto mb-3" />
          <h3 className="text-2xl font-bold text-white mb-2">Error Loading Metrics</h3>
          <p className="text-red-400 mb-4">{error}</p>
          <button 
            onClick={fetchMetrics}
            className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg"
          >
            Retry
          </button>
        </div>
      </div>
    );
  }
  const requestsTrend = formatTrend(metrics.requestsTrend);
  const responseTimeTrend = formatTrend(metrics.responseTimeTrend, true); // Lower is better
  const cacheHitTrend = formatTrend(metrics.cacheHitTrend);

  return (
    <div className="bg-gray-800 rounded-xl p-8 border border-gray-600">
      <div className="text-center mb-8">
        <h3 className="text-3xl font-bold text-white mb-3 flex items-center justify-center gap-3">
          <BarChart3 className="w-8 h-8 text-blue-500" />
          Gateway Metrics Dashboard
        </h3>
        <div className="flex items-center justify-center gap-2 text-gray-300">
          <span>Real-time performance indicators</span>
          <RefreshCw className="w-4 h-4 animate-spin" />
          <span className="text-sm">
            Updated {lastUpdated ? new Date(lastUpdated).toLocaleTimeString() : 'now'}
          </span>
        </div>
      </div>

      {/* Primary Metrics */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-6 mb-8">
        <div className="bg-gradient-to-br from-blue-900 to-blue-800 p-6 rounded-xl text-center border border-blue-600 shadow-lg">
          <Activity className="w-8 h-8 text-blue-400 mx-auto mb-3" />
          <div className="text-3xl font-bold text-blue-400 mb-2">
            {metrics.requestsPerMinute.toLocaleString()}
          </div>
          <div className="text-sm text-blue-200 uppercase tracking-wide mb-2">Requests/min</div>
          <div className={`text-xs ${requestsTrend.colorClass} flex items-center justify-center gap-1`}>
            <span>{requestsTrend.arrow} {requestsTrend.text}</span>
            <span className="text-blue-300">vs last hour</span>
          </div>
        </div>

        <div className="bg-gradient-to-br from-green-900 to-green-800 p-6 rounded-xl text-center border border-green-600 shadow-lg">
          <Clock className="w-8 h-8 text-green-400 mx-auto mb-3" />
          <div className="text-3xl font-bold text-green-400 mb-2">{metrics.averageResponseTimeMs}ms</div>
          <div className="text-sm text-green-200 uppercase tracking-wide mb-2">Avg Response</div>
          <div className={`text-xs ${responseTimeTrend.colorClass} flex items-center justify-center gap-1`}>
            <span>{responseTimeTrend.arrow} {responseTimeTrend.text}</span>
            <span className="text-green-300">performance</span>
          </div>
        </div>

        <div className="bg-gradient-to-br from-yellow-900 to-yellow-800 p-6 rounded-xl text-center border border-yellow-600 shadow-lg">
          <Database className="w-8 h-8 text-yellow-400 mx-auto mb-3" />
          <div className="text-3xl font-bold text-yellow-400 mb-2">{metrics.cacheHitRatePercentage}%</div>
          <div className="text-sm text-yellow-200 uppercase tracking-wide mb-2">Cache Hit Rate</div>
          <div className={`text-xs ${cacheHitTrend.colorClass} flex items-center justify-center gap-1`}>
            <span>{cacheHitTrend.arrow} {cacheHitTrend.text}</span>
            <span className="text-yellow-300">efficiency</span>
          </div>
        </div>

        <div className="bg-gradient-to-br from-red-900 to-red-800 p-6 rounded-xl text-center border border-red-600 shadow-lg">
          <Zap className="w-8 h-8 text-red-400 mx-auto mb-3" />
          <div className="text-3xl font-bold text-red-400 mb-2">{metrics.rateLimitedRequests}</div>
          <div className="text-sm text-red-200 uppercase tracking-wide mb-2">Rate Limited</div>
          <div className="text-xs text-red-300 flex items-center justify-center gap-1">
            <span>last 5min</span>
          </div>
        </div>
      </div>

      {/* Secondary Metrics */}
      <div className="grid grid-cols-2 md:grid-cols-6 gap-4 mb-8">
        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <Shield className="w-6 h-6 text-purple-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-purple-400 mb-1">
            {metrics.authRequests > 1000 ? `${(metrics.authRequests/1000).toFixed(1)}K` : metrics.authRequests}
          </div>
          <div className="text-xs text-gray-400 uppercase">Auth Requests</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <Users className="w-6 h-6 text-cyan-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-cyan-400 mb-1">{metrics.activeUsers.toLocaleString()}</div>
          <div className="text-xs text-gray-400 uppercase">Active Users</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <AlertTriangle className="w-6 h-6 text-orange-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-orange-400 mb-1">{metrics.circuitBreakersOpen}</div>
          <div className="text-xs text-gray-400 uppercase">Circuit Open</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <BarChart3 className="w-6 h-6 text-indigo-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-indigo-400 mb-1">{metrics.uptimePercentage}%</div>
          <div className="text-xs text-gray-400 uppercase">Uptime</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <Database className="w-6 h-6 text-emerald-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-emerald-400 mb-1">{metrics.cacheSizeMB}MB</div>
          <div className="text-xs text-gray-400 uppercase">Cache Size</div>
        </div>

        <div className="bg-gray-900 p-4 rounded-lg text-center border border-gray-600">
          <Activity className="w-6 h-6 text-pink-400 mx-auto mb-2" />
          <div className="text-xl font-bold text-pink-400 mb-1">{metrics.activeServices}</div>
          <div className="text-xs text-gray-400 uppercase">Services</div>
        </div>
      </div>

      {/* Status Summary */}
      <div className="bg-gray-900 rounded-xl p-6 border border-gray-600">
        <h4 className="text-lg font-bold text-white mb-4 text-center">System Health Summary</h4>
        
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div className="text-center">
            <div className={`text-2xl font-bold mb-1 ${metrics.overallPerformanceScore >= 90 ? 'text-green-400' : metrics.overallPerformanceScore >= 70 ? 'text-yellow-400' : 'text-red-400'}`}>
              {metrics.overallPerformanceScore >= 90 ? 'Excellent' : metrics.overallPerformanceScore >= 70 ? 'Good' : 'Needs Attention'}
            </div>
            <div className="text-sm text-gray-400 mb-2">Overall Performance</div>
            <div className="w-full bg-gray-700 rounded-full h-2">
              <div 
                className={`h-2 rounded-full ${metrics.overallPerformanceScore >= 90 ? 'bg-green-400' : metrics.overallPerformanceScore >= 70 ? 'bg-yellow-400' : 'bg-red-400'}`}
                style={{ width: `${metrics.overallPerformanceScore}%` }}
              ></div>
            </div>
            <div className={`text-xs mt-1 ${metrics.overallPerformanceScore >= 90 ? 'text-green-400' : metrics.overallPerformanceScore >= 70 ? 'text-yellow-400' : 'text-red-400'}`}>
              {metrics.overallPerformanceScore}% optimal
            </div>
          </div>

          <div className="text-center">
            <div className={`text-2xl font-bold mb-1 ${metrics.loadBalancingEfficiency >= 85 ? 'text-blue-400' : metrics.loadBalancingEfficiency >= 70 ? 'text-yellow-400' : 'text-red-400'}`}>
              {metrics.loadBalancingEfficiency >= 85 ? 'Stable' : metrics.loadBalancingEfficiency >= 70 ? 'Moderate' : 'Unstable'}
            </div>
            <div className="text-sm text-gray-400 mb-2">Load Balancing</div>
            <div className="w-full bg-gray-700 rounded-full h-2">
              <div 
                className={`h-2 rounded-full ${metrics.loadBalancingEfficiency >= 85 ? 'bg-blue-400' : metrics.loadBalancingEfficiency >= 70 ? 'bg-yellow-400' : 'bg-red-400'}`}
                style={{ width: `${metrics.loadBalancingEfficiency}%` }}
              ></div>
            </div>
            <div className={`text-xs mt-1 ${metrics.loadBalancingEfficiency >= 85 ? 'text-blue-400' : metrics.loadBalancingEfficiency >= 70 ? 'text-yellow-400' : 'text-red-400'}`}>
              {metrics.loadBalancingEfficiency}% efficiency
            </div>
          </div>

          <div className="text-center">
            <div className={`text-2xl font-bold mb-1 ${metrics.errorRatePercentage <= 0.5 ? 'text-green-400' : metrics.errorRatePercentage <= 2 ? 'text-yellow-400' : 'text-red-400'}`}>
              {metrics.errorRatePercentage <= 0.5 ? 'Excellent' : metrics.errorRatePercentage <= 2 ? 'Good' : 'High'}
            </div>
            <div className="text-sm text-gray-400 mb-2">Error Rate</div>
            <div className="w-full bg-gray-700 rounded-full h-2">
              <div 
                className={`h-2 rounded-full ${metrics.errorRatePercentage <= 0.5 ? 'bg-green-400' : metrics.errorRatePercentage <= 2 ? 'bg-yellow-400' : 'bg-red-400'}`}
                style={{ width: `${Math.min(metrics.errorRatePercentage * 10, 100)}%` }}
              ></div>
            </div>
            <div className={`text-xs mt-1 ${metrics.errorRatePercentage <= 0.5 ? 'text-green-400' : metrics.errorRatePercentage <= 2 ? 'text-yellow-400' : 'text-red-400'}`}>
              {metrics.errorRatePercentage}% errors
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}