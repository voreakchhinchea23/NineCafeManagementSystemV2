export const RATE = 4000 // riel per USD, same as the API's report rate

export const riel = (n) => Math.round(Number(n) || 0).toLocaleString('en-US') + ' ៛'
export const usd = (n) => '$' + ((Number(n) || 0) / RATE).toFixed(2)
export const dateTime = (v) => (v ? new Date(v).toLocaleString() : '-')
export const todayStr = () => new Date().toISOString().slice(0, 10)
