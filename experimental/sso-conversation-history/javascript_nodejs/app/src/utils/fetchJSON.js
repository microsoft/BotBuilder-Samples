export default async function fetchJSON(url, options) {
  const res = await fetch(
    url,
    {
      ...options,
      headers: {
        ...(options || {}).headers,
        accept: 'application/json'
      }
    }
  );

  if (!res.ok) {
    throw new Error(`Server returned ${ res.status }`);
  }

  return await res.json();
}
