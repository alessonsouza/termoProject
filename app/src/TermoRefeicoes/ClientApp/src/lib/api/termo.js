import api from './api';

const TermoAPI = {
  termosAceitos: async (values) => {
    let returnFromApi;
    await api
      .post(`/termos-aceitos/signatures`, {
        NUMCAD: values.NUMCAD,
        DATA_INICIO: values.DATA_INICIO,
        DATA_FIM: values.DATA_FIM,
        CODCCU: values.CODCCU,
        JAHACEITOU: values.JAHACEITOU,
      })
      .then((res) => {
        returnFromApi = res;
      });
    return returnFromApi.data;
  },
  setores: async () => {
    const returnFromApi = await api.get('/termos-aceitos/setores');
    return returnFromApi;
  },
  getTermo: async () => {
    const returnFromApi = await api.get('/termo/get-termo');
    return returnFromApi;
  },
  excel: async (values) => {

    const returnFromApi = await api.post(`/termos-aceitos/documento-excel`, {
      NUMCAD: values.NUMCAD,
      DATA_INICIO: values.DATA_INICIO,
      DATA_FIM: values.DATA_FIM,
      CODCCU: values.CODCCU,
      JAHACEITOU: values.JAHACEITOU,
    }, { responseType: 'blob' });


    return returnFromApi.data;
  },
};

export default TermoAPI;
