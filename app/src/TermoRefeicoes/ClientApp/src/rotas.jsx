import React, { Suspense } from 'react';
import { Route, Switch } from 'react-router-dom';

import PrivateRoute from './components/routes/private-route';
import DefaultRoute from './components/routes/public-route';
import Loader from './components/loader';
import Home from './pages/default';
import TermosAceitos from './pages/reports/termos-aceitos';
import ConsumosAceitos from './pages/reports/consumos-aceitos';
import Lancamentos from './pages/reports/lancamentos';
import Configs from './pages/configs';
import Login from './pages/login';

// const TermosAceitos = lazy(() => import('./pages/reports/termos-aceitos'));

const Rotas = () => {
  return (
    <Suspense
      fallback={
        <div>
          <Loader loading />
        </div>
      }>
      <Switch>
        <DefaultRoute exact path="/login" component={Login} />
        <PrivateRoute
          path="/relatorios/termos"
          component={() => <TermosAceitos />}
        />
        <PrivateRoute
          path="/relatorios/consumos"
          component={() => <ConsumosAceitos />}
        />
        <PrivateRoute
          path="/relatorios/lancamentos"
          component={() => <Lancamentos />}
        />
        <PrivateRoute path="/configuracoes" component={() => <Configs />} />
        {/*
          <PrivateRoute path="/restrito" component={PaginaRestrita} />
        */}
        <PrivateRoute path="/" component={() => <Home />} />
        <Route path="*" component={() => <h1>Page not found</h1>} />
      </Switch>
    </Suspense>
  );
};
export default Rotas;
