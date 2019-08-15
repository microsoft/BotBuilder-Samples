import Context from '../Context';
import hocContext from '../../utils/hocContext';

// Composing a component in HOC manner and send the user name as prop
export default (selector = state => state) => hocContext(
  Context,
  ({ name }) => selector({ name })
)
