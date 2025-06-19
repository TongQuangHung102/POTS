export const isValidString = (str) => {
    const regex = /^[a-zA-Z0-9]+$/;
    return regex.test(str);
  };

export const validateEmail = (email) => {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
};

export const cleanAndValidateString = (str) => {
    const normalized = str.replace(/\s{2,}/g, ' ').trim();

    if (normalized === '') return { valid: false, cleaned: '' };
  
    const specialCharRegex = /[^a-zA-Z0-9À-ỹà-ỹ\s]/u;
    if (specialCharRegex.test(normalized)) return { valid: false, cleaned: normalized };
  
    return { valid: true, cleaned: normalized };
  };

  export const normalizeName = (name) => {
    if (!name) return ''; 

    return name
        .trim()
        .replace(/\s+/g, ' '); 
};
  