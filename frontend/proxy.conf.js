const backendApiUrl = process.env.API_URL || 'http://localhost:5001';

module.exports = {
  '/api/**': {
    target: backendApiUrl,
    secure: false,
    changeOrigin: true,
  },
};
