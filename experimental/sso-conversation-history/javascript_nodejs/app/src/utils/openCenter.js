// Adopted from https://stackoverflow.com/questions/4068373/center-a-popup-window-on-screen.
export default function openCenter(url, title, popupWidth, popupHeight) {
  const dualScreenLeft = typeof window.screenLeft !== 'undefined' ? window.screenLeft : window.screenX;
  const dualScreenTop = typeof window.screenTop !== 'undefined' ? window.screenTop : window.screenY;

  const width = window.innerWidth || document.documentElement.clientWidth || window.screen.width;
  const height = window.innerHeight || document.documentElement.clientHeight || window.screen.height;

  const systemZoom = width / window.screen.availWidth;
  const left = (width - popupWidth) / 2 / systemZoom + dualScreenLeft;
  const top = (height - popupHeight) / 2 / systemZoom + dualScreenTop;

  const features = {
    height: popupHeight / systemZoom,
    left,
    scrollbars: 'yes',
    top,
    width: popupWidth / systemZoom
  };

  return window.open(
    url,
    title,
    Object.keys(features).map(key => `${ key }=${ features[key] }`).join(',')
  );
}
