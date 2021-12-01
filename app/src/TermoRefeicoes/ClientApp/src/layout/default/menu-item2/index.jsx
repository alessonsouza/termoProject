import React from 'react';
import PropTypes from 'prop-types';
import { Link, useLocation } from 'react-router-dom';

const MenuItem2 = (props) => {
  const { label, link, disabled } = props;

  const location = useLocation();

  const classActive = location.pathname === link ? 'active' : '';
  const classDisabled = disabled ? 'disabled' : '';

  return (
    <li className={`nav-item ${classActive}`}>
      <Link
        to={link}
        className={`nav-link ${classDisabled}`}
        aria-disabled={disabled}>
        {label}{' '}
      </Link>
    </li>
  );
};

MenuItem2.propTypes = {
  label: PropTypes.string.isRequired,
  link: PropTypes.string.isRequired,
  disabled: PropTypes.bool,
};

MenuItem2.defaultProps = {
  disabled: false,
};

export default MenuItem2;
