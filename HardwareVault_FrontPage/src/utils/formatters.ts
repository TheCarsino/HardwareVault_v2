// utils/formatters.ts
/**
 * Converts RAM from MB to GB
 */
export const ramToGB = (mb: number): string => {
  return `${Math.round(mb / 1024)} GB`;
};

/**
 * Formats storage size with type
 */
export const formatStorage = (gb: number, type: string): string => {
  if (gb >= 1000) {
    return `${(gb / 1000).toFixed(1)} TB ${type}`;
  }
  return `${gb} GB ${type}`;
};

/**
 * Formats weight in kg
 */
export const formatWeight = (kg: number): string => {
  return `${kg.toFixed(2)} kg`;
};

/**
 * Formats power supply wattage
 */
export const formatWatts = (watts: number): string => {
  return `${watts} W`;
};

/**
 * Formats USB port count
 */
export const formatUsbPorts = (count: number): string => {
  return count === 1 ? '1 port' : `${count} ports`;
};

/**
 * Formats date using date-fns
 */
export const formatDate = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
};

/**
 * Formats date with time
 */
export const formatDateTime = (dateString: string): string => {
  const date = new Date(dateString);
  return date.toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
};

/**
 * Formats relative time (e.g., "2 hours ago")
 */
export const formatRelativeTime = (dateString: string): string => {
  const date = new Date(dateString);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffMins = Math.floor(diffMs / 60000);
  const diffHours = Math.floor(diffMins / 60);
  const diffDays = Math.floor(diffHours / 24);

  if (diffMins < 1) return 'Just now';
  if (diffMins < 60) return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
  if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
  if (diffDays < 30) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
  
  return formatDate(dateString);
};

/**
 * Formats import success percentage
 */
export const formatSuccessRate = (successCount: number, totalCount: number): string => {
  if (totalCount === 0) return '0%';
  const percentage = Math.round((successCount / totalCount) * 100);
  return `${percentage}%`;
};
