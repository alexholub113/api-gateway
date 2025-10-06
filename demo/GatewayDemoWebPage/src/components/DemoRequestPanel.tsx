import axios, { AxiosError } from 'axios';
import { Activity, AlertCircle, CheckCircle, Clock, Play, Settings, XCircle } from 'lucide-react';
import React, { useState } from 'react';

interface DemoResponseModel {
  statusCode: number;
  statusText: string;
  responseTime: number;
  body: string;
  headers: Record<string, string>;
}

interface DemoRequestPanelProps {
  className?: string;
}

export const DemoRequestPanel: React.FC<DemoRequestPanelProps> = ({ className = '' }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [lastResponse, setLastResponse] = useState<DemoResponseModel | null>(null);
  const [progress, setProgress] = useState(0);
  
  // Custom request settings
  const [customMethod, setCustomMethod] = useState('GET');
  const [targetServiceId, setTargetServiceId] = useState('target-service');
  const [selectedEndpoint, setSelectedEndpoint] = useState('/users/list');
  const [customUserId, setCustomUserId] = useState('1');
  const [customOrderId, setCustomOrderId] = useState('123');

  const getEndpointUrl = () => {
    switch (selectedEndpoint) {
      case '/users/list':
        return 'https://localhost:7214/route/users/list';
      case '/users/{id}':
        return `https://localhost:7214/route/users/${customUserId}`;
      case '/orders/{id}/details':
        return `https://localhost:7214/route/orders/${customOrderId}/details`;
      case '/products/list':
        return 'https://localhost:7214/route/products/list';
      default:
        return 'https://localhost:7214/route/users/list';
    }
  };

  const sendCustomRequest = async () => {
    setIsLoading(true);
    setLastResponse(null);
    setProgress(0);

    // Simulate progress
    const progressInterval = setInterval(() => {
      setProgress(prev => {
        if (prev >= 90) {
          clearInterval(progressInterval);
          return 90;
        }
        return prev + 10;
      });
    }, 100);

    const startTime = Date.now();

    try {
      const response = await axios({
        method: customMethod.toLowerCase(),
        url: getEndpointUrl(),
        headers: {
          'X-Gateway-TargetServiceId': targetServiceId,
          'Content-Type': 'application/json'
        },
        timeout: 30000,
        validateStatus: () => true // Accept all status codes
      });

      const endTime = Date.now();
      const responseTime = endTime - startTime;

      clearInterval(progressInterval);
      setProgress(100);

      const demoResponse: DemoResponseModel = {
        statusCode: response.status,
        statusText: response.statusText,
        responseTime: responseTime,
        body: typeof response.data === 'string' ? response.data : JSON.stringify(response.data, null, 2),
        headers: response.headers as Record<string, string>
      };

      setTimeout(() => {
        setLastResponse(demoResponse);
        setIsLoading(false);
        setProgress(0);
      }, 200);
    } catch (error: unknown) {
      console.error('Error sending custom request:', error);
      clearInterval(progressInterval);
      setProgress(100);

      const endTime = Date.now();
      const responseTime = endTime - startTime;

      // Create error response with proper type checking
      const axiosError = error as AxiosError;
      const errorResponse: DemoResponseModel = {
        statusCode: axiosError.response?.status || 0,
        statusText: axiosError.response?.statusText || 'Network Error',
        responseTime: responseTime,
        body: JSON.stringify(axiosError.response?.data || axiosError.message || 'Request failed', null, 2),
        headers: (axiosError.response?.headers as Record<string, string>) || {}
      };

      setTimeout(() => {
        setLastResponse(errorResponse);
        setIsLoading(false);
        setProgress(0);
      }, 200);
    }
  };

  const getStatusBadgeColor = (statusCode: number) => {
    if (statusCode < 300) return 'bg-green-600 text-white';
    if (statusCode < 400) return 'bg-yellow-600 text-white';
    return 'bg-red-600 text-white';
  };

  const getStatusIcon = (statusCode: number) => {
    if (statusCode < 300) return <CheckCircle className="w-4 h-4" />;
    if (statusCode < 400) return <AlertCircle className="w-4 h-4" />;
    return <XCircle className="w-4 h-4" />;
  };

  return (
    <div className={`bg-gray-800 rounded-xl p-8 border border-gray-600 ${className}`}>
      {/* Header */}
      <div className="text-center mb-8">
        <h3 className="text-3xl font-bold text-white mb-3 flex items-center justify-center gap-3">
          <Play className="w-8 h-8 text-blue-500" />
          Demo Request Simulator
        </h3>
        <p className="text-lg text-gray-300">Test the API Gateway capabilities with interactive request simulation</p>
      </div>

      {/* Content */}
      <div className="space-y-8">
        {/* Custom Request Builder */}
        <div className="bg-gray-700 rounded-xl p-6 border border-gray-600">
          <h4 className="text-xl font-semibold text-white mb-4 flex items-center gap-3">
            <Settings className="w-6 h-6 text-green-500" />
            Request Configuration
          </h4>
          <div className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">HTTP Method:</label>
                <select
                  value={customMethod}
                  onChange={(e) => setCustomMethod(e.target.value)}
                  className="w-full px-3 py-2 bg-gray-800 border border-gray-600 rounded-lg text-white 
                           focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                >
                  <option value="GET">GET</option>
                  <option value="POST">POST</option>
                  <option value="PUT">PUT</option>
                  <option value="DELETE">DELETE</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">Target Service ID:</label>
                <input
                  type="text"
                  value={targetServiceId}
                  onChange={(e) => setTargetServiceId(e.target.value)}
                  placeholder="target-service"
                  className="w-full px-3 py-2 bg-gray-800 border border-gray-600 rounded-lg text-white 
                           focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">Endpoint:</label>
                <select
                  value={selectedEndpoint}
                  onChange={(e) => setSelectedEndpoint(e.target.value)}
                  className="w-full px-3 py-2 bg-gray-800 border border-gray-600 rounded-lg text-white 
                           focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                >
                  <option value="/users/list">Users List</option>
                  <option value="/users/{id}">User by ID</option>
                  <option value="/products/list">Products List</option>
                  <option value="/orders/{id}/details">Order Details</option>
                </select>
              </div>
            </div>

            {/* Dynamic Parameters */}
            {selectedEndpoint === '/users/{id}' && (
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">User ID:</label>
                <input
                  type="number"
                  value={customUserId}
                  onChange={(e) => setCustomUserId(e.target.value)}
                  placeholder="1"
                  className="w-full px-3 py-2 bg-gray-800 border border-gray-600 rounded-lg text-white 
                           focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            )}

            {selectedEndpoint === '/orders/{id}/details' && (
              <div>
                <label className="block text-sm font-medium text-gray-300 mb-2">Order ID:</label>
                <input
                  type="number"
                  value={customOrderId}
                  onChange={(e) => setCustomOrderId(e.target.value)}
                  placeholder="123"
                  className="w-full px-3 py-2 bg-gray-800 border border-gray-600 rounded-lg text-white 
                           focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
            )}

            <div>
              <label className="block text-sm font-medium text-gray-300 mb-2">Request URL:</label>
              <div className="bg-gray-900 p-4 rounded-lg border border-gray-600">
                <code className="text-blue-400 font-mono text-sm break-all">{getEndpointUrl()}</code>
              </div>
              <p className="text-xs text-gray-400 mt-2">This endpoint will be called with the specified target service ID header</p>
            </div>

            <div className="flex gap-4">
              <button
                onClick={sendCustomRequest}
                disabled={isLoading}
                className="bg-green-600 hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed 
                         text-white px-8 py-3 rounded-lg font-medium transition-all duration-200 
                         hover:transform hover:-translate-y-0.5 hover:shadow-lg flex items-center gap-2"
              >
                <Play className="w-5 h-5" />
                Send Request
              </button>
              
              <div className="flex items-center gap-2 text-gray-400">
                <Activity className="w-4 h-4" />
                <span className="text-sm">Gateway will route to configured target service</span>
              </div>
            </div>
          </div>
        </div>

        {/* Progress Bar */}
        {isLoading && (
          <div className="bg-gray-700 rounded-lg p-4 border border-gray-600">
            <h4 className="text-lg font-semibold text-white mb-3 flex items-center gap-2">
              <Clock className="w-5 h-5 text-blue-600" />
              Processing Request
            </h4>
            <div className="w-full bg-gray-600 rounded-full h-2 mb-2">
              <div 
                className="bg-blue-600 h-2 rounded-full transition-all duration-300" 
                style={{ width: `${progress}%` }}
              ></div>
            </div>
            <p className="text-sm text-gray-300 text-center">{progress}% Complete</p>
          </div>
        )}

        {/* Results Display */}
        {lastResponse && (
          <div className="bg-gray-700 rounded-lg p-4 border border-gray-600">
            <h4 className="text-lg font-semibold text-white mb-3 flex items-center gap-2">
              <Activity className="w-5 h-5 text-blue-600" />
              Response Result
            </h4>
            <div className="bg-gray-800 rounded-lg border border-gray-600 overflow-hidden">
              <div className="flex justify-between items-center p-4 bg-gray-900 border-b border-gray-600">
                <div className={`flex items-center gap-2 px-3 py-1 rounded-lg ${getStatusBadgeColor(lastResponse.statusCode)}`}>
                  {getStatusIcon(lastResponse.statusCode)}
                  <span className="font-medium">{lastResponse.statusCode} {lastResponse.statusText}</span>
                </div>
                <div className="flex items-center gap-2 text-gray-300">
                  <Clock className="w-4 h-4" />
                  <span className="font-mono">{lastResponse.responseTime}ms</span>
                </div>
              </div>
              <div className="p-4">
                <pre className="text-sm text-gray-300 font-mono whitespace-pre-wrap overflow-x-auto">
                  {lastResponse.body}
                </pre>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};