import axios from 'axios';
import { Activity, CheckCircle, Clock, RefreshCw, XCircle } from 'lucide-react';
import React, { useEffect, useState } from 'react';

interface InstanceHealthResponse {
  address: string;
  weight: number;
  isHealthy: boolean;
  lastChecked: string;
}

interface ServiceHealthResponse {
  serviceId: string;
  loadBalancingStrategy: string;
  totalInstances: number;
  healthyInstances: number;
  rateLimitPolicy: string | null;
  cachePolicy: string | null;
  instances: InstanceHealthResponse[];
}

interface ServiceHealthMonitorProps {
  className?: string;
}

export const ServiceHealthMonitor: React.FC<ServiceHealthMonitorProps> = ({ className = '' }) => {
  const [services, setServices] = useState<ServiceHealthResponse[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [lastUpdated, setLastUpdated] = useState<Date | null>(null);
  const [error, setError] = useState<string | null>(null);

  const fetchHealthStatus = async () => {
    try {
      setIsLoading(true);
      setError(null);
      
      const response = await axios.get<ServiceHealthResponse[]>('https://localhost:7214/health-status', {
        timeout: 10000,
        validateStatus: () => true
      });

      if (response.status === 200) {
        setServices(response.data);
        setLastUpdated(new Date());
      } else {
        setError(`Failed to fetch health status: ${response.status} ${response.statusText}`);
      }
    } catch (err: unknown) {
      console.error('Error fetching health status:', err);
      const error = err as { message?: string };
      setError(error.message || 'Failed to fetch health status');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    // Initial fetch
    fetchHealthStatus();

    // Set up interval to refresh every 15 seconds
    const interval = setInterval(fetchHealthStatus, 15000);

    return () => clearInterval(interval);
  }, []);

  const getHealthStatusColor = (isHealthy: boolean) => {
    return isHealthy ? 'text-green-600' : 'text-red-600';
  };

  const getHealthStatusIcon = (isHealthy: boolean) => {
    return isHealthy ? <CheckCircle className="w-4 h-4" /> : <XCircle className="w-4 h-4" />;
  };

  const formatLastUpdated = (date: Date) => {
    return date.toLocaleTimeString();
  };

  return (
    <div className={`bg-gray-800 rounded-xl p-6 border border-gray-600 ${className}`}>
      {/* Header */}
      <div className="text-center mb-6">
        <h3 className="text-2xl font-bold text-white mb-2 flex items-center justify-center gap-2">
          <Activity className="w-6 h-6 text-green-600" />
          Service Health Monitor
        </h3>
        <p className="text-gray-300">Real-time health status of all gateway services</p>
        {lastUpdated && (
          <p className="text-xs text-gray-400 mt-2 flex items-center justify-center gap-1">
            <Clock className="w-3 h-3" />
            Last updated: {formatLastUpdated(lastUpdated)}
          </p>
        )}
      </div>

      {/* Refresh Button */}
      <div className="flex justify-center mb-4">
        <button
          onClick={fetchHealthStatus}
          disabled={isLoading}
          className="bg-blue-600 hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed 
                   text-white px-4 py-2 rounded-lg font-medium transition-all duration-200 
                   flex items-center gap-2"
        >
          <RefreshCw className={`w-4 h-4 ${isLoading ? 'animate-spin' : ''}`} />
          {isLoading ? 'Refreshing...' : 'Refresh Now'}
        </button>
      </div>

      {/* Error Display */}
      {error && (
        <div className="bg-red-900 border border-red-600 rounded-lg p-4 mb-4">
          <div className="flex items-center gap-2 text-red-200">
            <XCircle className="w-5 h-5" />
            <span className="font-medium">Error:</span>
          </div>
          <p className="text-red-300 mt-1 text-sm">{error}</p>
        </div>
      )}

      {/* Services Display */}
      {services.length === 0 && !isLoading && !error ? (
        <div className="text-center text-gray-400 py-8">
          <Activity className="w-12 h-12 mx-auto mb-3 opacity-50" />
          <p>No services configured</p>
        </div>
      ) : (
        <div className="space-y-4">
          {services.map((service) => (
            <div key={service.serviceId} className="bg-gray-700 rounded-lg p-4 border border-gray-600">
              {/* Service Header */}
              <div className="flex justify-between items-center mb-4">
                <div>
                  <h4 className="text-lg font-semibold text-white">{service.serviceId}</h4>
                  <p className="text-sm text-gray-400">
                    Strategy: {service.loadBalancingStrategy} | 
                    Instances: {service.healthyInstances}/{service.totalInstances} healthy
                  </p>
                  {(service.rateLimitPolicy || service.cachePolicy) && (
                    <div className="flex gap-2 mt-1">
                      {service.rateLimitPolicy && (
                        <span className="text-xs bg-yellow-600 text-white px-2 py-1 rounded">
                          Rate: {service.rateLimitPolicy}
                        </span>
                      )}
                      {service.cachePolicy && (
                        <span className="text-xs bg-blue-600 text-white px-2 py-1 rounded">
                          Cache: {service.cachePolicy}
                        </span>
                      )}
                    </div>
                  )}
                </div>
                <div className="text-right">
                  <div className={`text-2xl font-bold ${service.healthyInstances === service.totalInstances ? 'text-green-600' : 'text-yellow-600'}`}>
                    {Math.round((service.healthyInstances / service.totalInstances) * 100)}%
                  </div>
                  <div className="text-xs text-gray-400">Health</div>
                </div>
              </div>

              {/* Instances */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                {service.instances.map((instance, index) => (
                  <div 
                    key={`${instance.address}-${index}`} 
                    className={`bg-gray-800 rounded-lg p-3 border-l-4 ${
                      instance.isHealthy ? 'border-green-600' : 'border-red-600'
                    }`}
                  >
                    <div className="flex justify-between items-start">
                      <div className="flex-1">
                        <div className="font-mono text-sm text-white mb-1">
                          {instance.address}
                        </div>
                        <div className="text-xs text-gray-400">
                          Weight: {instance.weight}
                        </div>
                      </div>
                      <div className={`flex items-center gap-1 ${getHealthStatusColor(instance.isHealthy)}`}>
                        {getHealthStatusIcon(instance.isHealthy)}
                        <span className="text-sm font-medium">
                          {instance.isHealthy ? 'Healthy' : 'Unhealthy'}
                        </span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};