import api from './api';

const ConfigAPI = {
  getConfigs: async () => {
    const returnFromApi = await api.get('/config/get-config');
    return returnFromApi.data;
  },
  saveConfigs: async (valor) => {
    let returnFromApi;
    await api.post('/config/save-config', valor).then(resp => returnFromApi = resp);
    return returnFromApi.data;
  },
};

export default ConfigAPI;
