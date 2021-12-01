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
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Snackbar,
} from '@material-ui/core';
import { Alert, AlertTitle } from '@material-ui/lab';
import MuiAlert from '@material-ui/lab/Alert';
import { Box, BoxTitle } from '../../components/box-card';
import RefeicoesAPI from '../../lib/api/refeicoes';
import TermoAPI from '../../lib/api/termo';
import TokenAPI from '../../lib/api/token';
import './pagina1.css';
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
  const vertical = 'bottom';
  const horizontal = 'center';
  console.log(Texto);
  const Message = (props) => {
    return <MuiAlert elevation={6} variant="filled" {...props} />;
  };

  const GetDate = (type) => {
    let dateMes = Number(mes);
    let dateAno = Number(ano);

    dateMes = type === 'previous' ? Number(mes) - 1 : Number(mes) + 1;
    if (dateMes < 1) {
      dateMes = 12;
      dateAno -= 1;
    }

    if (dateMes > 12) {
      dateMes = 1;
      dateAno += 1;
    }
    // Refeicoes(dateAno + dateMes.toString().padStart(2, '0'));
    setTypeAction(type);
    setMes(dateMes);
    setAno(dateAno);
  };

  const FormatarHora = (valor) => {
    const hora = valor.substr(0, 2);
    let min = valor.substr(3, 4);
    const dh = hora * 60;
    min *= 1;
    return dh + min;
  };

  const Refeicoes = async (data) => {
    const termo = await RefeicoesAPI.getTermoAccept(
      dadosUser?.matricula || storage?.matricula,
    );
    setTermoAccept(termo.data > 0);
    if (termo.data > 0) {
      const count = await RefeicoesAPI.countAccept(data);
      const resp = await RefeicoesAPI.allRefeicoes(data);
      setTypeAction(termo.lastMonth);
      setNotAccept(count);
      setDados(resp);
    }
  };

  const ConfirmarConsumos = async (matricula, competencia) => {
    const hora = await FormatarHora(
      dayjs().tz('America/Sao_Paulo').format('HH:mm'),
    );
    const resp = await RefeicoesAPI.confirmarConsumos(
      matricula,
      competencia,
      hora,
    );
    if (resp.data.success) {
      setSuccess(true);
    }
  };

  const GetTermo = async () => {
    const resp = await TermoAPI.getTermo();
    if (resp?.data) {
      setTermoDesc(resp.data.data[0]);
      console.log(termoDesc);
    }
  };

  const AceitarTermo = async () => {
    // const texto = document.getElementById('text-term').innerHTML;

    const data = {};
    const hora = await FormatarHora(
      dayjs().tz('America/Sao_Paulo').format('HH:mm'),
    );
    data.NUMCAD = dadosUser?.matricula || storage?.matricula;
    data.HORA_ACEITE = hora;
    data.DATA_ACEITE = dayjs().tz('America/Sao_Paulo').format();
    data.TERMO_DESCRICAO = document.getElementById('text-term').innerText;
    data.FK_TERMO = termoDesc.usU_CODVER;

    const resp = await RefeicoesAPI.submitTerm(data);
    if (resp?.success) {
      setTypeMessageTerm('success');
      setSuccess(true);
    } else {
      setTypeMessageTerm('error');
      setSuccess(true);
    }
    setMessageTerm(resp.error);
  };

  const RenderHeader = () => {
    return (
      <thead>
        <tr key={1} className="bg-cinza-claro ml-5 text-left">
          <th scope="col" className="mt-2" style={{ maxWidth: '50px' }}>
            Item
          </th>
          <th scope="col" style={{ maxWidth: '50px' }}>
            Data/Hora
          </th>
          <th scope="col size120">Dia da semana</th>
          <th scope="col size120">Qtd</th>
          <th scope="col size120">Valor</th>
        </tr>
      </thead>
    );
  };

  const FormataDinheiro = (valor) => {
    if (!valor) {
      valor = 0;
    }
    let value = 'R$ ';
    value += valor
      .toFixed(2)
      .replace('.', ',')
      .replace(/(\d)(?=(\d{3})+,)/g, '$1.');
    return value;
  };

  const FormataHora = (valor) => {
    const hora = Math.trunc(valor / 60);
    const min = Math.abs(hora * 60 - valor);
    let dh = hora.toString().padStart(2, '0');
    dh += ':';
    dh += min.toString().padStart(2, '0');
    return dh;
  };

  const formataDia = (data) => {
    let dayOfWeek;
    switch (data) {
      case 0:
        dayOfWeek = 'Segunda-feira';
        break;
      case 1:
        dayOfWeek = 'Terça-feira';
        break;
      case 2:
        dayOfWeek = 'Quarta-feira';
        break;
      case 3:
        dayOfWeek = 'Quinta-feira';
        break;
      case 4:
        dayOfWeek = 'Sexta-feira';
        break;
      case 5:
        dayOfWeek = 'Sábado';
        break;
      case 6:
        dayOfWeek = 'Domingo';
        break;

      default:
        break;
    }
    return dayOfWeek;
  };

  const Snacks = () => {
    const snack = [];
    let key = 0;
    if (dados.data?.length > 0) {
      dados.data.map((item, i) => {
        if (item.codRefeicao > 5) {
          key += i === 0 ? 0 : 1;
          snack[key] = item;
        }
        return snack;
      });
    }

    return (
      snack &&
      snack.map((items, i) => {
        const statusColor = items.usu_Datchk ? 'success' : 'warning';
        const status = items.usu_Datchk ? 'Aceito' : 'Pendente';
        const number = +i;
        const data = new Date(dayjs(items.dataRef).format('YYYY-MM-DD'));
        const weekDay = formataDia(data.getDay());
        return (
          <>
            <tr key={number}>
              <th scope="row" style={{ maxWidth: '158px' }}>
                {items.descRefeicao}{' '}
                <span className={`badge badge-${statusColor}`}>{status}</span>
              </th>
              <td>
                {dayjs(items.dataRef).format('DD/MM/YYYY')}{' '}
                {FormataHora(items.horaCC)}
              </td>
              <td className="text-left">{weekDay}</td>
              <td>{items.qtdAcc}</td>
              <td>{FormataDinheiro(items.valorRef)}</td>
            </tr>
          </>
        );
      })
    );
  };

  const Lunchs = () => {
    const lunch = [];
    let key = 0;
    if (dados.data?.length > 0) {
      dados.data.map((item, i) => {
        if (item.codRefeicao <= 5) {
          key += i === 0 ? 0 : 1;
          lunch[key] = item;
        }
        return lunch;
      });
    }
    return (
      lunch &&
      lunch.map((items, i) => {
        const statusColor = items.usu_Datchk ? 'success' : 'warning';
        const status = items.usu_Datchk ? 'Aceito' : 'Pendente';
        const number = +i;
        const data = new Date(dayjs(items.dataRef).format('YYYY-MM-DD'));
        const weekDay = formataDia(data.getDay());

        return (
          <tr key={number}>
            <th scope="row" style={{ maxWidth: '120px' }}>
              {items.descRefeicao}{' '}
              <span className={`badge badge-${statusColor}`}>{status}</span>
            </th>
            <td>
              {dayjs(items.dataRef).format('DD/MM/YYYY')}{' '}
              {FormataHora(items.horaCC)}
            </td>
            <td>{weekDay}</td>
            <td>{items.qtdAcc}</td>
            <td>{FormataDinheiro(items.valorRef)}</td>
          </tr>
        );
      })
    );
  };

  const RenderItems = () => {
    let result = 0;
    let result1 = 0;
    let result2 = 0;
    const snacks = dados.data?.length > 0 ? Snacks() : [];
    const lunch = dados.data?.length > 0 ? Lunchs() : [];
    if (dados.data) {
      dados?.data.forEach((item) => {
        result += item.valorRef;
        if (item.codRefeicao <= 5) {
          result1 += item.valorRef;
        } else {
          result2 += item.valorRef;
        }
      });
    }
    return (
      <tbody className="text-left">
        {lunch}
        {lunch.length > 0 && (
          <tr key={111111111}>
            <th scope="row"> </th>
            <td className="text-right" />
            <td className="text-right" />
            <td className="size120 text-right">
              <b>Total Refeições:</b>
            </td>
            <td className="size120 text-right">
              <b>{FormataDinheiro(result1)}</b>
            </td>
          </tr>
        )}
        {snacks.length > 0 && (
          <tr key={22222222}>
            <th scope="row"> </th>
            <td className="text-center" />
            <td className="text-left">
              <h4>
                <b>Frigobar</b>
              </h4>
            </td>
            <td className="size120" />
            <td className="size120" />
          </tr>
        )}

        {snacks.length > 0 && snacks}

        {result2 > 0 && (
          <tr key={snacks.length}>
            <th scope="row"> </th>
            <td className="text-center" />
            <td className="text-center" />
            <td className="size120 text-right">
              <b>Total Frigobar:</b>
            </td>
            <td className="size120 text-right">
              <b>{FormataDinheiro(result2)}</b>
            </td>
          </tr>
        )}
        {result > 0 && (
          <tr key={result}>
            <th scope="row"> </th>
            <td className="text-center" />
            <td className="text-center" />
            <td className="size120 text-right">
              <h4>
                <b>Total:</b>
              </h4>
            </td>
            <td className="size120 text-right">
              <h4>
                <b>{FormataDinheiro(result)}</b>
              </h4>
            </td>
          </tr>
        )}
      </tbody>
    );
  };

  const handleClose = () => {
    setDialogTerm(false);
    setSuccess(false);
  };

  const CanAcceptTerm = () => {
    const begin = `${ano}-${mes.toString().padStart(2, '0')}-26`;
    const end = `${ano}-${mes.toString().padStart(2, '0')}-31`;
    const result = dayjs(day).isBetween(dayjs(begin), dayjs(end), null, '[)');
    setCanNotAccept(result);
  };

  const paramPut = ano + mes.toString().padStart(2, '0');

  useEffect(() => {
    Refeicoes(ano + mes.toString().padStart(2, '0'));

    // CheckAcceptedTerm();
    CanAcceptTerm();
  }, [mes, ano, success]);

  useEffect(() => {
    const dia = dayjs().tz('America/Sao_Paulo').format('DD');
    if (dia >= 27) {
      let nextMonth = parseInt(mes, 10);
      nextMonth += 1;
      Refeicoes(ano + nextMonth.toString().padStart(2, '0'));
      setMes(nextMonth.toString().padStart(2, '0'));
    }
  }, []);

  useEffect(() => {
    GetTermo();
  }, []);

  const habilitaBotao = () => {
    const hoje = parseInt(dayjs().tz('America/Sao_Paulo').format('DD'), 10);
    const mesAtual = parseInt(dayjs().tz('America/Sao_Paulo').format('MM'), 10);
    const anoAtual = parseInt(
      dayjs().tz('America/Sao_Paulo').format('YYYY'),
      10,
    );

    if (parseInt(mes, 10) === mesAtual && hoje <= 25) {
      return true;
    }
    if (parseInt(mes, 10) === mesAtual && hoje >= 27) {
      return false;
    }

    if (parseInt(mes, 10) === mesAtual && parseInt(ano, 10) >= anoAtual) {
      return true;
    }
    if (parseInt(mes, 10) > mesAtual && hoje >= 27) {
      return true;
    }
    return false;
  };

  const Render = () => {
    let usuDatchk;
    let usuHorchk;
    if (dados.data) {
      dados.data.map((item) => {
        usuDatchk = item.usu_Datchk;
        usuHorchk = item.usu_Horchk;
        return usuDatchk;
      });
    }

    return (
      <Box>
        <div className="col-md-12 cor-laranja text-center">
          <h2>
            Colaborador: {dadosUser?.matricula || storage?.matricula}-
            {dadosUser?.name || storage?.name}
          </h2>
        </div>
        <BoxTitle />
        <div className="row mb-3" />
        <div className="row text-center mb-3">
          <div className="col-md-4">
            <button
              disabled={
                dayjs(typeAction).format('MM/YYYY') ===
                `${mes.toString().padStart(2, '0')}/${ano}`
              }
              type="button"
              className="btn btn-lg btn-success bg-verde-primario"
              onClick={() => GetDate('previous')}>
              Mês Anterior
            </button>
          </div>
          <div className="col-md-4 text-center">
            <h3>
              <b>
                {mes.toString().padStart(2, '0')}/{ano}
              </b>
            </h3>
          </div>
          <div className="col-md-4 ">
            <button
              disabled={
                habilitaBotao()
                // dayjs().format('MM') < mes.toString().padStart(2, '0') &&
                // dayjs().format('DD') <= 25
              }
              type="button"
              className="btn btn-lg btn-success bg-verde-primario"
              onClick={() => GetDate('next')}>
              Próximo Mês
            </button>
          </div>
          {!usuDatchk ? (
            <div className="col-md-12 cor-pink text-center">
              <h6>
                *Caso identifique alguma divergência nos consumos, contate a
                nutrição:
              </h6>
              <p>
                <b>Ramal:</b> 1877 - <b>Email:</b>{' '}
                consumosnd@unimedchapeco.coop.br
              </p>
            </div>
          ) : (
            <div className="col-md-12 cor-primaria text-center">
              <h6>
                *Termo aceito em {dayjs(usuDatchk).format('DD/MM/YYYY')}{' '}
                {FormataHora(usuHorchk)}, não sendo mais possivel realizar
                alterações!
              </h6>
            </div>
          )}
        </div>
        {notAccept > 0 && canNotAccept ? (
          <div>
            <Card>
              <CardContent>
                <div className="col-md-12 cor-primaria text-center">
                  <h3>
                    <b>Termo:</b>
                  </h3>
                  Eu <b> {dadosUser?.name || storage?.name}</b>, autorizo o
                  desconto em minha Folha de Pagamento das Refeições/Consumos
                  abaixo detalhados.
                </div>
                <div className="col-md-12 mt-2 text-center">
                  <button
                    type="button"
                    className="btn btn-lg btn-success bg-verde-unimed"
                    onClick={() => setDialogTerm(true)}>
                    Aceitar termo de responsabilidade sobre os consumos!
                  </button>
                </div>
              </CardContent>
            </Card>

            <Dialog
              open={dialogTerm}
              fullWidth
              onClose={handleClose}
              aria-labelledby="customized-dialog-title">
              <DialogContent dividers>
                <DialogTitle>
                  Tem certeza que verificou todos os seus consumos?
                </DialogTitle>
                {success && (
                  <Alert>
                    <AlertTitle>Termo aceito com sucesso!</AlertTitle>
                  </Alert>
                )}
              </DialogContent>
              <DialogActions>
                <button
                  type="button"
                  className="btn btn-lg btn-success bg-verde-unimed"
                  onClick={() =>
                    ConfirmarConsumos(
                      dadosUser?.matricula || storage?.matricula,
                      paramPut,
                    )
                  }>
                  Sim, verifiquei todos!
                </button>
                <button
                  type="button"
                  className="btn btn-lg btn-danger"
                  onClick={() => handleClose()}>
                  Sair
                </button>
              </DialogActions>
            </Dialog>
          </div>
        ) : null}
        <Card className="mb-4">
          <CardContent>
            <div className="row">
              <div className="col-md-12 table-responsive">
                <table className="table table-hover" key="id">
                  {RenderHeader()}
                  {RenderItems()}
                </table>
              </div>
            </div>
          </CardContent>
        </Card>
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
        <Dialog
          open={!termoAccept}
          maxWidth="md"
          onClose={handleClose}
          aria-labelledby="customized-dialog-title">
          <DialogTitle>
            TERMO DE RESPONSABILIDADE DE UTILIZAÇÃO DO SISTEMA ELETRÔNICO DE
            CONTROLE DE REFEIÇÃO!
          </DialogTitle>
          <DialogContent
            style={{
              textJustify: 'inter-word',
              textAlign: 'justify',
            }}>
            <Texto data={termoDesc?.usU_DESTER} />
          </DialogContent>
          <DialogActions>
            <button
              type="button"
              className="btn btn-lg btn-success bg-verde-unimed"
              onClick={() => AceitarTermo()}>
              Concordo!
            </button>
          </DialogActions>
        </Dialog>
      </Box>
    );
  };
  return Render();
};

export default Home;
