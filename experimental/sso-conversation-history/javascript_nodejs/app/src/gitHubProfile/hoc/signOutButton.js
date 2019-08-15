import Context from '../Context';
import hocContext from '../../utils/hocContext';

// Composing a component in HOC manner and send the sign out logic as prop
export default (selector = state => state) => hocContext(
  Context,
  ({ onSignOut }) => selector({ onClick: onSignOut })
)
