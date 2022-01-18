/* eslint-disable consistent-return */
/* eslint-disable array-callback-return */
import React, { useEffect, useState, useContext } from 'react';
import dayjs from 'dayjs';
import isBetween from 'dayjs/plugin/isBetween';
import ReactPaginate from 'react-paginate';

import { Card, CardContent, TextField, Fab, Divider } from '@material-ui/core';

import Icon from '@mdi/react';
import { mdiMagnify, mdiPrinter } from '@mdi/js';
import { Markup } from 'interweave';
import { Box, BoxTitle } from '../../../components/box-card';
import DatePicker from '../../../components/datePicker';
import SelectField from '../../../components/selectField';
import TermosAPI from '../../../lib/api/termo';
import SetorAPI from '../../../lib/api/setor';
import './termosAceitos.css';
import '../../../assets/css/colors.css';
import { LoaderContext } from '../../../lib/context/loader-context';

const Home = () => {
  dayjs.extend(isBetween);

  // const mes = dayjs().format('MM');
  // const ano = dayjs().format('YYYY');
  const [dados, setDados] = useState([]);
  const start = dayjs().startOf('month');
  const end = dayjs().endOf('month');
  const [setor, setSetor] = useState([]);
  const [enabledDates, setEnabledDates] = useState(true);
  const [filtros, setFiltros] = useState({
    dataInicio: start,
    dataFim: end,
    matricula: 0,
    setor: [],
    tPesquisa: 'aceitos',
  });

  const { setIsLoading } = useContext(LoaderContext);
  const [pageNumber, setPageNumber] = useState(0);
  const [termoDesc, setTermoDesc] = useState(null);

  const usersPerPage = 10;
  const pagesVisited = pageNumber * usersPerPage;
  const pageCount = Math.ceil(dados.length / usersPerPage);

  const changePage = ({ selected }) => {
    setPageNumber(selected);
  };

  const tipoPesquisa = [
    { id: 'aceitos', descricao: 'Aceitos' },
    { id: 'naoaceitos', descricao: 'Não Aceitos' },
  ];

  const GetTermo = async () => {
    const resp = await TermosAPI.getTermo();
    if (resp?.data) {
      setTermoDesc(resp.data.data[0]);
    }
  };

  const formataMonth = (datas) => {
    let dayOfWeek;
    switch (datas) {
      case 0:
        dayOfWeek = 'Janeiro';
        break;
      case 1:
        dayOfWeek = 'Fevereiro';
        break;
      case 2:
        dayOfWeek = 'Março';
        break;
      case 3:
        dayOfWeek = 'Abril';
        break;
      case 4:
        dayOfWeek = 'Maio';
        break;
      case 5:
        dayOfWeek = 'Junho';
        break;
      case 6:
        dayOfWeek = 'Julho';
        break;
      case 7:
        dayOfWeek = 'Agosto';
        break;
      case 8:
        dayOfWeek = 'Setembro';
        break;
      case 9:
        dayOfWeek = 'Outubro';
        break;
      case 10:
        dayOfWeek = 'Novembro';
        break;
      case 11:
        dayOfWeek = 'Dezembro';
        break;
      default:
        break;
    }
    return dayOfWeek;
  };

  const FormatarTermo = (dadosUsers) => {
    const day = dadosUsers.data.substr(0, 2);
    const month = formataMonth(dadosUsers.data.substr(4, 1));
    const year = dadosUsers.data.substr(6, 4);
    let term = termoDesc?.usU_DESTER.replace('%name', dadosUsers.name);
    term = term?.replace('%matricula', dadosUsers.matricula);
    term = term?.replace('%dia', day);
    term = term?.replace('%mes', month);
    term = term?.replace('%ano', year);
    return term?.substr(0, 150);
  };

  const onChange = (name, value) => {
    const campos = { ...filtros };
    if (name === 'tPesquisa') {
      setDados([]);
      if (value === 'naoaceitos') {
        campos.dataInicio = 'Invalid Date';
        campos.dataFim = 'Invalid Date';
        setEnabledDates(false);
      } else {
        campos.dataInicio = start;
        campos.dataFim = end;
        setEnabledDates(true);
      }
    }
    campos[name] = value;
    setFiltros(campos);
  };

  const GetSetores = async () => {
    const resp = await SetorAPI.setores();
    if (resp?.data.success === true) {
      setSetor(resp?.data.data);
    }
  };

  const Excel = async () => {
    setIsLoading(true);
    const data = {};
    data.NUMCAD = filtros?.matricula || 0;
    // data.DATA_INICIO =
    //   filtros?.dataInicio === 'Invalid Date'
    //     ? ' '
    //     : dayjs(filtros?.dataInicio).format('DD/MM/YYYY');
    // data.DATA_FIM =
    //   filtros?.dataFim === 'Invalid Date'
    //     ? ' '
    //     : dayjs(filtros?.dataFim).format('DD/MM/YYYY');
    data.JAHACEITOU = filtros?.tPesquisa.toString();
    if (data.JAHACEITOU === 'aceitos' || !data.JAHACEITOU) {
      // data.DATA_INICIO = ano + mes;
      // data.DATA_FIM = ano + mes;
      data.DATA_INICIO =
        filtros?.dataInicio === 'Invalid Date'
          ? ' '
          : dayjs(filtros?.dataInicio).format('YYYY') +
            dayjs(filtros?.dataInicio).format('MM') +
            dayjs(filtros?.dataInicio).format('DD');
      data.DATA_FIM =
        filtros?.dataFim === 'Invalid Date'
          ? ' '
          : dayjs(filtros?.dataFim).format('YYYY') +
            dayjs(filtros?.dataFim).format('MM') +
            dayjs(filtros?.dataFim).format('DD');
    } else {
      data.DATA_INICIO = ' ';
      data.DATA_FIM = ' ';
    }
    data.CODCCU = filtros?.setor.toString();
    const resp = await TermosAPI.excel(data);

    const blob = new Blob([resp], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });

    const link = document.createElement('a');
    const url = window.URL.createObjectURL(blob);

    link.href = url;
    link.download = 'Termos.xlsx';
    document.body.appendChild(link);
    console.log(link);
    link.click();
    window.URL.revokeObjectURL(url);
    setIsLoading(false);
  };

  const handleToggle = () => {
    Excel();
  };

  const AceitarTermo = async () => {
    setIsLoading(true);
    const data = {};
    data.NUMCAD = filtros?.matricula || 0;
    // data.DATA_INICIO =
    //   filtros?.dataInicio === 'Invalid Date'
    //     ? ' '
    //     : dayjs(filtros?.dataInicio).format('DD/MM/YYYY');
    // data.DATA_FIM =
    //   filtros?.dataFim === 'Invalid Date'
    //     ? ' '
    //     : dayjs(filtros?.dataFim).format('DD/MM/YYYY');
    data.JAHACEITOU = filtros?.tPesquisa.toString();
    if (data.JAHACEITOU === 'aceitos' || !data.JAHACEITOU) {
      // data.DATA_INICIO = ano + mes;
      // data.DATA_FIM = ano + mes;
      data.DATA_INICIO =
        filtros?.dataInicio === 'Invalid Date'
          ? ' '
          : dayjs(filtros?.dataInicio).format('YYYY') +
            dayjs(filtros?.dataInicio).format('MM') +
            dayjs(filtros?.dataInicio).format('DD');
      data.DATA_FIM =
        filtros?.dataFim === 'Invalid Date'
          ? ' '
          : dayjs(filtros?.dataFim).format('YYYY') +
            dayjs(filtros?.dataFim).format('MM') +
            dayjs(filtros?.dataFim).format('DD');
    } else {
      data.DATA_INICIO = ' ';
      data.DATA_FIM = ' ';
    }
    data.CODCCU = filtros?.setor.toString();
    const resp = await TermosAPI.termosAceitos(data);
    if (resp?.data) {
      setDados(resp?.data);
    }

    setIsLoading(false);
  };

  const RenderHeader = () => {
    return (
      <thead>
        <tr key={1} className="bg-cinza-claro ml-5 text-left">
          <th scope="col size120" className="mt-2">
            Matrícula
          </th>
          <th scope="col size120">Setor</th>
          <th scope="col" style={{ maxWidth: '50px' }}>
            Nome
          </th>
          <th scope="col size120">Data confirmação</th>
          <th scope="col size120">Descrição do termo</th>
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

  const RenderFiltros = () => {
    const setorField = {
      type: 'select',
      name: 'setor',
      label: 'Setor',
      items: setor || [],
      value: filtros?.setor,
      config: { text: 'descricao', value: 'id' },
      multiple: true,
    };
    const tipo = {
      name: 'tPesquisa',
      label: 'Tipo de pesquisa',
      items: tipoPesquisa || [],
      value: filtros?.tPesquisa || [],
      config: { text: 'descricao', value: 'id' },
    };
    const setorFieldIcon = {
      type: 'select',
      name: 'setor',
      label: 'Setor',
      items: setor || [],
      value: filtros?.setor || [],
      multiple: true,
    };
    const dataInicio = {
      name: 'dataInicio',
      label: 'Data Início',
      value: filtros?.dataInicio || '',
      format: 'YYYY-MM-DDTHH:mm-03:00',
    };

    const dataFim = {
      name: 'dataFim',
      label: 'Data Fim',
      value: filtros?.dataFim || '',
      format: 'YYYY-MM-DDTHH:mm-03:00',
    };

    return (
      <Card style={{ width: '100%' }}>
        <CardContent>
          <div className="row " style={{ justifyContent: 'center' }}>
            <div className="col-md-2">
              <TextField
                label="Matricula"
                name="Matricula"
                onChange={(v) => onChange('matricula', v.target.value)}
                fullWidth
              />
            </div>
            <div className="col-md-3">
              <SelectField
                data={setorField}
                onChange={onChange}
                icon={
                  setorFieldIcon.items && setorFieldIcon.items.length > 0
                    ? setorFieldIcon
                    : null
                }
              />
            </div>
            <div className="col-md-2">
              <SelectField data={tipo} onChange={onChange} />
            </div>
            {enabledDates && (
              <div className="col-md-2">
                <DatePicker data={dataInicio} onChange={onChange} />
              </div>
            )}
            {enabledDates && (
              <div className="col-md-2">
                <DatePicker data={dataFim} onChange={onChange} />
              </div>
            )}
            <div className="col-md-1 text-right">
              <Fab
                className="ml-25 mt-2"
                style={{ backgroundColor: '#005128' }}
                onClick={() => AceitarTermo()}>
                <Icon
                  path={mdiMagnify}
                  title="Pesquisar"
                  size={1}
                  color="white"
                />
              </Fab>
              <Fab
                className="ml-25 mt-2 bg-cinza-escuro"
                style={{ backgroundColor: '#79787d' }}
                onClick={() => handleToggle()}>
                <Icon
                  // className="cor-cinza-escuro"
                  path={mdiPrinter}
                  title="Pesquisar"
                  size={1}
                  color="white"
                />
              </Fab>
            </div>
          </div>
        </CardContent>
      </Card>
    );
  };

  const RenderDados = () => {
    const dadosUsers = {};
    return (
      dados &&
      dados.slice(pagesVisited, pagesVisited + usersPerPage).map((items, i) => {
        const year = dayjs(items?.datA_ACEITE).year();
        const statusColor =
          items.datA_ACEITE && year !== 1901 ? 'success' : 'warning';
        const status =
          items.datA_ACEITE && year !== 1901 ? 'Aceito' : 'Pendente';
        const number = +i;
        dadosUsers.name = items.nomfun;
        dadosUsers.matricula = items.numcad;

        dadosUsers.data =
          items.datA_ACEITE && year !== 1901
            ? dayjs(items?.datA_ACEITE).format('DD/MM/YYYY')
            : '';

        return (
          <tr key={number}>
            <th scope="row" style={{ maxWidth: '120px' }}>
              {items.numcad}{' '}
              <span className={`badge badge-${statusColor}`}>{status}</span>
            </th>
            <th scope="row" style={{ maxWidth: '120px' }}>
              {items.nomccu}
            </th>
            <td>{items.nomfun}</td>
            <td>
              {items.datA_ACEITE && year !== 1901
                ? dayjs(items?.datA_ACEITE).format('DD/MM/YYYY')
                : ''}{' '}
              {items.horA_ACEITE > 0 ? FormataHora(items.horA_ACEITE) : ''}
            </td>
            {/* <td>{items.termO_DESCRICAO?.substr(0, 180) || ''} ...</td> */}
            <td>
              {items.termO_DESCRICAO && (
                <Markup content={FormatarTermo(dadosUsers)} />
              )}
            </td>
          </tr>
        );
      })
    );
  };

  const RenderItems = () => {
    let result = 0;
    let result1 = 0;
    let result2 = 0;
    const snacks = RenderFiltros();
    const lunch = RenderDados();
    if (dados.data) {
      dados?.data.forEach((item) => {
        result += item.valorRef;
        if (item.codRefeicao === 2) {
          result1 += item.valorRef;
        } else {
          result2 += item.valorRef;
        }
      });
    }
    console.log(result1);
    return (
      <tbody className="text-left">
        {lunch}
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
  useEffect(() => {
    GetSetores();
    GetTermo();
  }, []);

  useEffect(() => {
    AceitarTermo();
  }, [filtros.tPesquisa]);

  const Render = () => {
    const filtrosPesqusia = RenderFiltros();
    return (
      <Box>
        <BoxTitle className="text-center">
          <h2>Termos</h2>
        </BoxTitle>
        <div className="row mb-3" />
        <div className="row text-center  p-5">
          {filtrosPesqusia}
          <Divider />
        </div>
        <Card className="mb-4">
          <CardContent>
            <div className="row">
              <div className="col-md-12 table-responsive">
                <table className="table table-hover" key="id">
                  {RenderHeader()}
                  {RenderItems()}
                </table>
                <div>
                  <ReactPaginate
                    previousLabel="Anterior"
                    nextLabel="Próximo"
                    pageCount={pageCount}
                    onPageChange={changePage}
                    containerClassName="paginationBttns"
                    previousLinkClassName="previousBttn"
                    nextLinkClassName="nextBttn"
                    disabledClassName="paginationDisabled"
                    activeClassName="paginationActive"
                  />
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
        {/* <Dialog
          open={!termoAccept}
          fullWidth
          onClose={handleClose}
          aria-labelledby="customized-dialog-title">
          <DialogTitle>Termos e condições gerais de uso!</DialogTitle>
          <DialogContent
            style={{ textJustify: 'inter-word', textAlign: 'justify' }}>
            {text}
          </DialogContent>
          <DialogActions>
            <button
              type="button"
              className="btn btn-lg btn-success bg-verde-termo"
              onClick={() => AceitarTermo()}>
              Aceito!
            </button>
            <button
              type="button"
              className="btn btn-lg btn-danger"
              onClick={() => handleClose()}>
              Sair
            </button>
          </DialogActions>
        </Dialog>{' '} */}
      </Box>
    );
  };
  return Render();
};

export default Home;
