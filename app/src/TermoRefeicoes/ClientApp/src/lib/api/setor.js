import api from './api';

const SetorAPI = {
  setores: async () => {
    const returnFromApi = await api.get('/termos-aceitos/setores');
    return returnFromApi;
  },
};

export default SetorAPI;
