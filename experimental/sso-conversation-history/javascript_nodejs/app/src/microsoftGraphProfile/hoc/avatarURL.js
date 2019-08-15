import Context from '../Context';
import hocContext from '../../utils/hocContext';

// Composing a component in HOC manner and send the avatar URL as prop
export default (selector = state => state) => hocContext(
  Context,
  ({ avatarURL }) => selector({ avatarURL })
)
