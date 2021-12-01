import api from './api';

const ConsumosPI = {
  ConsumosAceitos: async (values) => {
    let returnFromApi;
    await api
      .post(`/consumos-aceitos/consumption`, {
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

    const returnFromApi = await api.post(`/consumos-aceitos/documento-excel`, {
      NUMCAD: values.NUMCAD,
      DATA_INICIO: values.DATA_INICIO,
      DATA_FIM: values.DATA_FIM,
      CODCCU: values.CODCCU,
      JAHACEITOU: values.JAHACEITOU,
    }, { responseType: 'blob' });


    return returnFromApi.data;
  },
};

export default ConsumosPI;
