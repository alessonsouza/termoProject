/* eslint-disable array-callback-return */
import { React, useContext } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import MenuItem from './menu-item';
import MenuItem2 from './menu-item2';
import TokenAPI from '../../lib/api/token';
import { AuthContext } from '../../lib/context/auth-context';
import { DropDown, DropDownItem, Divider } from './dropdown';

import './menu.css';

const Menu = () => {
  const { dadosUser } = useContext(AuthContext);
  const storage = TokenAPI.getToken();
  const groups = dadosUser?.groups || storage?.groups;
  const RemoveStorage = () => {
    TokenAPI.removeToken();
  };
  let relatoriosRH = false;
  let relatoriosNutri = false;
  let ShowRelatorios = false;

  groups?.map((e) => {
    if (
      e.groupName === 'G_RH' ||
      e.groupName === 'SW_RH' ||
      e.groupName === 'SW_TI' ||
      e.groupName === 'G_TI'
    ) {
      relatoriosRH = true;
    }
    if (
      e.groupName === 'G_NUTRICAO' ||
      e.groupName === 'SW_NUTRICAO' ||
      e.groupName === 'SW_TI' ||
      e.groupName === 'G_TI'
    ) {
      relatoriosNutri = true;
    }
  });

  if (relatoriosNutri || relatoriosRH) {
    ShowRelatorios = true;
  }
  return (
    <>
      {ShowRelatorios && (
        <>
          <div
            className="col-md-3 navbar-nav"
            style={{ marginTop: '1%', justifyContent: 'flex-end' }}>
            <MenuItem2 label="Home" link="/" />
          </div>
          <div
            className="col-md-3 navbar-nav"
            style={{ justifyContent: 'flex-end' }}>
            <DropDown descricao="Relatórios">
              {relatoriosRH && (
                <>
                  {' '}
                  <Divider />
                  <DropDownItem descricao="Termos" link="/relatorios/termos" />
                  <Divider />
                  <DropDownItem
                    descricao=" Consumos"
                    link="/relatorios/consumos"
                  />
                  <Divider />{' '}
                </>
              )}
              {relatoriosNutri && (
                <>
                  <DropDownItem
                    descricao=" Lançamentos"
                    link="/relatorios/lancamentos"
                  />
                  <Divider />{' '}
                </>
              )}
              {relatoriosNutri && (
                <DropDownItem
                  descricao=" Configurações"
                  link="/configuracoes"
                />
              )}
            </DropDown>
          </div>
        </>
      )}

      <div className="col-md-3" style={{ marginTop: '1%', textAlign: 'end' }}>
        <p className="color">{dadosUser?.name || storage?.name}</p>
        <ul className="navbar-nav" style={{ justifyContent: 'flex-end' }}>
          <MenuItem label="Sair" link="/login" onClick={RemoveStorage} />
        </ul>
      </div>
    </>
  );
};

export default Menu;
