import api from './api';

const TermoAPI = {
  releases: async (values) => {
    let returnFromApi;
    await api
      .post(`/lancamentos/releases`, {
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
  excel: async (values) => {

    const returnFromApi = await api.post(`/lancamentos/documento-excel`,
      values, { responseType: 'blob' });


    return returnFromApi.data;
  },
};

export default TermoAPI;
