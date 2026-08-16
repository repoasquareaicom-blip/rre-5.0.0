const salemBaseUrl =
  import.meta.env.VITE_SALEM_API_URL?.trim() ||
  'https://salem.rreconnect.in'
export const branches = [
  {
    id: 'RR-SALEM',
    label: 'Salem',
    baseUrl: salemBaseUrl,
    accent: '#b91c1c',
    accentSoft: '#fee2e2',
    accentBorder: '#fca5a5',
  },
  {
    id: 'RR-NAMAKKAL',
    label: 'Namakkal',
    baseUrl: 'https://namakkal.rreconnect.in',
    accent: '#b45309',
    accentSoft: '#fef3c7',
    accentBorder: '#f4c56a',
  },
  {
    id: 'RR-KOLATHUR',
    label: 'Kolathur',
    baseUrl: 'https://kolathur.rreconnect.in',
    accent: '#0f766e',
    accentSoft: '#e0f2f1',
    accentBorder: '#99d8d0',
  },
]

export const defaultBranchId = 'RR-SALEM'

export function getBranchById(branchId) {
  return branches.find((branch) => branch.id === branchId) || branches[0]
}
