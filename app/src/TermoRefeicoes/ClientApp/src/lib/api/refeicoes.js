import api from './api';

const RefeicoesAPI = {
  allRefeicoes: async (values) => {
    const returnFromApi = await api.get(`/refeicoes/${values}`);
    return returnFromApi.data;
  },
  countAccept: async (values) => {
    const returnFromApi = await api.get(`/refeicoes/get-count/${values}`);
    return returnFromApi.data;
  },
  confirmarConsumos: async (matricula, competencia, hora) => {
    const returnFromApi = await api.put(`/termo/${competencia}/${matricula}/${hora}`);
    return returnFromApi;
  },
  getTermoAccept: async (values) => {
    const returnFromApi = await api.get(`/refeicoes/get-termo/${values}`);
    return returnFromApi.data;
  },
  submitTerm: async (values) => {
    let returnFromApi;
    await api
      .post(`/refeicoes/submit-termo`, {
        NUMCAD: values.NUMCAD,
        DATA_ACEITE: values.DATA_ACEITE,
        HORA_ACEITE: values.HORA_ACEITE,
        TERMO_DESCRICAO: values.TERMO_DESCRICAO,
        FK_TERMO: values.FK_TERMO,
      })
      .then((res) => {
        returnFromApi = res;
      });
    return returnFromApi.data;
  },
};

export default RefeicoesAPI;
