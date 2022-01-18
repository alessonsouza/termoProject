// import React, { useState } from 'react';
// // import Menu from './menu';

// import './header.css';

// const Header = () => {
//   const [estaAberto, setEstaAberto] = useState(false);

//   const mudaAbertoFechado = () => {
//     setEstaAberto(!estaAberto);
//   };

//   const classNavBarOpen = estaAberto ? 'show' : '';
//   const classNavButton = estaAberto ? '' : 'collapsed';

//   return (
//     <div className="row">
//       <div className="col-md-12">
//         <nav
//           className={`${classNavBarOpen} navbar navbar-expand-lg navbar-light bg-verde-escuro`}
//           style={{ height: '76px' }}>
//
//           <button
//             className={`navbar-toggler ${classNavButton}`}
//             type="button"
//             data-toggle="collapse"
//             data-target="#navbarSupportedContent"
//             aria-controls="navbarSupportedContent"
//             aria-expanded={estaAberto ? 'true' : 'false'}
//             aria-label="Toggle navigation"
//             onClick={() => mudaAbertoFechado()}>
//             <Menu />
//           </button>
//           {/* <div
//             className="col-md-6 collapse navbar-collapse"
//             style={{ justifyContent: 'flex-end', color: '#fff' }}>
//             <h2>Refeições</h2>
//           </div>

//           <div
//             className=" col-md-6 collapse navbar-collapse"
//             id="navbarSupportedContent"
//             style={{ justifyContent: 'flex-end', width: '0%' }}>
//             <Menu />
//           </div> */}
//         </nav>
//       </div>
//     </div>
//   );
// };

// export default Header;
import React, { useState } from 'react';
import Menu from './menu';

import './header.css';

const Header = () => {
  const [estaAberto, setEstaAberto] = useState(false);

  const mudaAbertoFechado = () => {
    setEstaAberto(!estaAberto);
  };

  const classNavBarOpen = estaAberto ? 'show' : '';
  const classNavButton = estaAberto ? '' : 'collapsed';

  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-verde-escuro">
      <button
        className={`navbar-toggler ${classNavButton}`}
        type="button"
        data-toggle="collapse"
        data-target="#navbarSupportedContent"
        aria-controls="navbarSupportedContent"
        aria-expanded={estaAberto ? 'true' : 'false'}
        aria-label="Toggle navigation"
        onClick={() => mudaAbertoFechado()}>
        <span className="line" />
        <span className="line" />
        <span className="line" />
      </button>

      <h2 style={{ textAlign: 'end', color: '#fff', width: '100%' }}>
        Refeições Nutrição
      </h2>
      <div
        style={{ justifyContent: 'flex-end' }}
        className={`col-md-5 collapse navbar-collapse  ${classNavBarOpen}`}
        id="navbarSupportedContent">
        <Menu />
      </div>
      {/* <div
          className="col-md-6 collapse navbar-collapse"
          style={{ justifyContent: 'flex-end', color: '#fff' }}>
            </div> */}
    </nav>
  );
};

export default Header;
