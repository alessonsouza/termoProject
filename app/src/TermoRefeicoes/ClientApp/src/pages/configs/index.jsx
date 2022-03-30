/* eslint-disable no-unused-vars */
/* eslint-disable consistent-return */
/* eslint-disable array-callback-return */
import React, { useContext, useEffect, useState } from 'react';

import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import timezone from 'dayjs/plugin/timezone';
import isBetween from 'dayjs/plugin/isBetween';

import {
  Card,
  CardContent,
  CardHeader,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Snackbar,
  TextField,
} from '@material-ui/core';
import { Alert, AlertTitle } from '@material-ui/lab';
import MuiAlert from '@material-ui/lab/Alert';
import { Box, BoxTitle } from '../../components/box-card';
import RefeicoesAPI from '../../lib/api/refeicoes';
import ConfigAPI from '../../lib/api/configs';
import TokenAPI from '../../lib/api/token';
import '../../assets/css/unimed.css';
import Texto from '../../assets/text-term';
import { AuthContext } from '../../lib/context/auth-context';

const Home = () => {
  dayjs.extend(utc);
  dayjs.extend(timezone);
  dayjs.extend(isBetween);

  const { dadosUser } = useContext(AuthContext);
  const storage = TokenAPI.getToken();
  const day = dayjs().tz('America/Sao_Paulo').format('YYYY-MM-DD');
  const [mes, setMes] = useState(dayjs().tz('America/Sao_Paulo').format('MM'));
  const [ano, setAno] = useState(
    dayjs().tz('America/Sao_Paulo').format('YYYY'),
  );
  const [dados, setDados] = useState([]);
  const [notAccept, setNotAccept] = useState(0);
  const [termoAccept, setTermoAccept] = useState(false);
  const [canNotAccept, setCanNotAccept] = useState(false);
  const [success, setSuccess] = useState(false);
  const [dialogTerm, setDialogTerm] = useState(false);
  const [typeAction, setTypeAction] = useState(null);
  const [termoDesc, setTermoDesc] = useState(null);
  const [messageTerm, setMessageTerm] = useState('');
  const [typeMessageTerm, setTypeMessageTerm] = useState('');
  const [diaInicio, setDiaInicio] = useState(1);
  const [diaFim, setDiaFim] = useState(1);
  const vertical = 'bottom';
  const horizontal = 'center';
  console.log(Texto);
  const Message = (props) => {
    return <MuiAlert elevation={6} variant="filled" {...props} />;
  };

  const onChange = (name, value) => {
    if (name === 'diaInicio') {
      setDiaInicio(value);
    } else {
      setDiaFim(value);
    }
  };

  const onSave = async () => {
    const obj = {};
    obj.diaInicio = parseInt(diaInicio, 10);
    obj.diaFim = parseInt(diaFim, 10);

    const resp = await ConfigAPI.saveConfigs(obj);
    if (resp.data === 0) {
      setSuccess(true);
      setTypeMessageTerm('success');
      setMessageTerm('Alterado com sucesso!');
    }
  };

  const GetTermo = async () => {
    const resp = await ConfigAPI.getConfigs();
    if (resp?.data) {
      setDiaInicio(resp?.data.diaInicio);
      setDiaFim(resp?.data.diaFim);
    }
  };

  useEffect(() => {
    GetTermo();
  }, []);

  const handleClose = () => {
    setSuccess(false);
  };

  const Render = () => {
    return (
      <>
        <Snackbar
          open={success}
          autoHideDuration={6000}
          onClose={handleClose}
          anchorOrigin={{ vertical, horizontal }}
          key={vertical + horizontal}>
          <Message color={typeMessageTerm} severity={typeMessageTerm}>
            {messageTerm}
          </Message>
        </Snackbar>
        <Box>
          <div className="col-md-12 cor-laranja text-center">
            <h2>Configurações</h2>
          </div>
          <BoxTitle />
          <Card>
            <div className="col-md-6">
              <h3>Periodo de aceite</h3>
            </div>
            <div className="row  mb-3">
              <div className="col-md-6">
                <TextField
                  label="Entre o dia "
                  variant="outlined"
                  value={diaInicio}
                  onChange={(e) => onChange('diaInicio', e.target.value)}
                />{' '}
                <TextField
                  label="E o dia"
                  variant="outlined"
                  value={diaFim}
                  onChange={(e) => onChange('diaFim', e.target.value)}
                />
              </div>
            </div>
          </Card>
          <Card className="mb-5">
            <div className="col-md-12 text-center mb-2">
              <button
                type="button"
                className="btn btn-lg btn-success bg-verde-primario"
                onClick={() => onSave()}>
                Salvar
              </button>
            </div>
          </Card>
        </Box>
      </>
    );
  };
  return Render();
};

export default Home;
