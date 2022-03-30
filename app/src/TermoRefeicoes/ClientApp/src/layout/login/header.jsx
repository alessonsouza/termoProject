import React from 'react';

import './header.css';

const Header = () => {
  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-verde-escuro">
      <a className="navbar-brand" href="#1">
        <img
          alt="Unimed Chapecó"
          src="https://unimedchapeco.coop.br/assets/img/logo_110_51.png"
        />
      </a>

      <div
        className="collapse navbar-collapse justify-content-center"
        id="navbarSupportedContent"
      />
    </nav>
  );
};

export default Header;
