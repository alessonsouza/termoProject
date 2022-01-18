/* eslint-disable max-len */
// import React from 'react';
import React, { useContext } from 'react';
import dayjs from 'dayjs';
import { Markup } from 'interweave';
import { AuthContext } from '../../lib/context/auth-context';
import TokenAPI from '../../lib/api/token';

const Text = (props) => {
  const { dadosUser } = useContext(AuthContext);
  const storage = TokenAPI.getToken();
  const user = dadosUser || storage;
  const day = dayjs().format('DD');
  const month = dayjs().month();
  const year = dayjs().year();

  const { data } = props;

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
  const FormatarTermo = () => {
    // const day = dadosUsers.data.substr(0, 2);
    // const month = dadosUsers.data.substr(3, 5);
    // const year = dadosUsers.data.substr(6, 9);
    const mes = formataMonth(month);
    let term = data?.replace('%name', user.name);
    term = term?.replace('%name', user.name);
    term = term?.replace('%matricula', user.matricula);
    term = term?.replace('%matricula', user.matricula);
    term = term?.replace('%dia', day);
    term = term?.replace('%mes', mes);
    term = term?.replace('%ano', year);

    return term;
  };

  return (
    <div id="text-term">
      <Markup content={FormatarTermo()} />
    </div>
  );
};

export default Text;
