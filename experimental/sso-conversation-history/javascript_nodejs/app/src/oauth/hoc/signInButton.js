import Context from '../Context';
import hocContext from '../../utils/hocContext';

// Composing a component in HOC manner and send the sign in logic as prop
export default (selector = state => state) => hocContext(
  Context,
  ({ onSignIn }) => selector({ onClick: onSignIn })
)
