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
      {/* <p>
        Eu, <b>%name</b>, matrícula funcional nº <b>%matricula</b>, declaro ter
        ciência da obrigatoriedade do cumprimento das regulamentações descritas
        no presente termo, assumindo total responsabilidade pela correta
        utilização do Sistema Eletrônico de Controle de Refeições da Unimed
        Chapecó - Cooperativa de Trabalho Médico da Região Oeste Catarinense,
        mediante as considerações e condições abaixo descritas:
      </p>
      <p>
        Considerando que a Unimed Chapecó é inscrita no PAT - Programa de
        Alimentação do Trabalhador, mantendo serviços próprios de fornecimento
        de refeições;
      </p>
      <p>
        Considerando que a Unimed Chapecó oferece cardápio/refeição aos seus
        colaboradores de acordo com a jornada de trabalho e respeitando disposto
        nas normas internas da Cooperativa;
      </p>
      <p>
        {' '}
        Considerando que as refeições, desde que realizadas de acordo com o
        cardápio oferecido (lanche da manhã, almoço, lanche da tarde e jantar),
        conforme jornada de trabalho realizada nas dependências do refeitório da
        Unimed Chapecó, custarão ao(à) colaborador(a) 20% (vinte por cento) do
        valor total da refeição;
      </p>
      <p>
        Considerando que o consumo de refeições/itens não previstos no cardápio
        oferecido pela Unimed Chapecó, não é subsidiado, e, portanto, custarão
        ao(à) colaborador(a) 100% (cem por cento) do valor correspondente ao
        total da refeição/item adquirido;
      </p>
      <p>
        Considerando que a Unimed Chapecó realiza o controle das refeições/itens
        consumidos através do Sistema Eletrônico de Controle de Refeição;
      </p>
      <p>
        Considerando que o lançamento em sistema das refeições/itens consumidos
        e seus respectivos valores, é realizado por colaborador do Setor de
        Nutrição da Unimed Chapecó no momento do consumo e validado através da
        identificação digital do(a) colaborador(a)consumidor(a);
      </p>

      <p>
        Considerando que os lançamentos de refeições/itens poderão ser acessados
        pelo(a) Colaborador(a) através do portal Intranet - Sistema Eletrônico
        de Controle de Refeição, através de login e senha individual, sigilosa e
        intransferível;
      </p>
      <p>
        <b>CLÁUSULA PRIMEIRA – </b>
        Pelo presente termo de responsabilidade e utilização, comprometo-me a:
      </p>
      <p>
        <b>Parágrafo Primeiro - </b>Utilizar o sistema eletrônico de forma
        correta, registrando toda e qualquer refeição e validando-a através de
        identificação digital no momento do efetivo consumo.
      </p>
      <p>
        <b>Parágrafo Segundo - </b>Apresentar crachá de identificação ao
        colaborador do Setor de Nutrição no caso de impossibilidade de
        utilização do sistema de identificação digital, possibilitando o correto
        registro da refeição/item consumido através da matrícula funcional.
      </p>
      <p>
        <b>Parágrafo Terceiro - </b>Informar corretamente minha matricula
        funcional quando a solicitação de refeições ocorrer de forma eletrônica
        através do preenchimento de formulário online.{' '}
      </p>
      <p>
        <b>Paragrafo Quarto - </b>Conferir no momento do consumo e/ou de forma
        digital através do Sistema Eletrônico de Controle de Refeição, os
        lançamentos efetuados pelo colaborador do Setor de Nutrição e validados
        através da identificação digital e/ou matricula funcional.{' '}
      </p>
      <p>
        <b>Paragrafo Quinto - </b>Contatar imediatamente o Setor de Nutrição,
        quando identificado algum lançamento indevido e/ou incorreto, para a
        devida conferência e alteração caso necessário.{' '}
      </p>
      <p>
        <b>CLÁUSULA SEGUNDA – </b>Declaro estar ciente e autorizo o desconto em
        folha de pagamento do valor correspondente a todas as refeições/itens
        lançados no sistema de Sistema Eletrônico de Controle de Refeição,
        relativos às refeições constantes no cardápio da Unimed Chapecó de
        acordo com as normas internas e jornada de trabalho (subsidiadas), bem
        como dos itens/refeições não previstos no mencionado cardápio (não
        subsidiados) e por mim consumidos.{' '}
      </p>
      <p>
        <b>Parágrafo Primeiro – </b>O período de apuração das despesas
        relacionadas aos consumos realizados no setor de nutrição e lançados no
        Sistema Eletrônico de Controle de Refeição, ocorrerá entre os dias 26 ao
        dia 25 de cada mês, sendo minha responsabilidade realizar a conferência
        e aceite dos itens lançados, impreterivelmente, até o dia 27 de cada
        mês, independentemente do dia da semana, não sendo possível a realização
        da conferência após a referida data.
      </p>
      <p>
        <b>Paragrafo Segundo - </b>Decorrido o prazo para conferência dos
        lançamentos, o valor total do consumo lançado no respectivo mês será
        descontado em folha de pagamento, independentemente da realização da
        conferência/aceite.
      </p>
      <p>
        <b>Paragrafo Terceiro – </b>Em casos de afastamento temporário por
        motivos de férias, auxílio doença, auxílio maternidade/ paternidade ou
        qualquer outro, minha identificação digital será bloqueada, não sendo
        possível a realização de refeições e/ou quaisquer outros consumos nas
        dependências do refeitório da Unimed Chapecó. Nada mais havendo e ciente
        das responsabilidades pelas obrigações estipuladas e autorizações
        concedidas, firmo o presente termo.
      </p>
      <p className="text-center"> Chapecó/SC, %dia de %mes de %ano.</p>
      <p className="text-center" style={{ marginBottom: '-18px' }}>
        {' '}
        %name
      </p> */}
    </div>
  );
};

export default Text;
