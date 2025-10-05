import axios from 'axios';

const API_BASE_URL = 'https://localhost:7214';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Types for demo requests
export interface DemoRequestModel {
  method: string;
  url: string;
  body?: string;
  headers?: Record<string, string>;
}

export interface DemoResponseModel {
  statusCode: number;
  statusText: string;
  responseTime: number;
  body: string;
  headers: Record<string, string>;
}

export interface TrafficGenerationModel {
  url: string;
  method: string;
  requestCount: number;
  delayMs: number;
}

export interface TrafficResultModel {
  totalRequests: number;
  successfulRequests: number;
  failedRequests: number;
  averageResponseTime: number;
  results: DemoResponseModel[];
}

export interface PredefinedRequestModel {
  name: string;
  method: string;
  url: string;
  description: string;
  body?: string;
}

// API Service Class
export class ApiService {
  // Send a demo request
  async sendRequest(request: DemoRequestModel): Promise<DemoResponseModel> {
    try {
      const response = await apiClient.post<DemoResponseModel>('/api/demo/send-request', request);
      return response.data;
    } catch (error) {
      console.error('Error sending demo request:', error);
      throw error;
    }
  }

  // Generate traffic for load testing
  async generateTraffic(model: TrafficGenerationModel): Promise<TrafficResultModel> {
    try {
      const response = await apiClient.post<TrafficResultModel>('/api/demo/generate-traffic', model);
      return response.data;
    } catch (error) {
      console.error('Error generating traffic:', error);
      throw error;
    }
  }

  // Get predefined demo requests
  async getPredefinedRequests(): Promise<PredefinedRequestModel[]> {
    try {
      const response = await apiClient.get<PredefinedRequestModel[]>('/api/demo/predefined-requests');
      return response.data;
    } catch (error) {
      console.error('Error fetching predefined requests:', error);
      throw error;
    }
  }

  // Get gateway metrics from Prometheus endpoint
  async getMetrics(): Promise<string> {
    try {
      const response = await apiClient.get<string>('/metrics', {
        headers: {
          'Accept': 'text/plain',
        },
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching metrics:', error);
      throw error;
    }
  }

  // Get service health status
  async getHealthStatus(): Promise<any> {
    try {
      const response = await apiClient.get('/health-status');
      return response.data;
    } catch (error) {
      console.error('Error fetching health status:', error);
      throw error;
    }
  }
}

// Export singleton instance
export const apiService = new ApiService();