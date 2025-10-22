/**
 * Format chatbot response by processing escape sequences and formatting
 * @param {string} text - Raw text from API response
 * @returns {string} - Formatted text
 */
export const formatChatbotResponse = (text) => {
  if (!text) return '';

  // Replace escape sequences
  let formatted = text
    // Replace \n with actual line breaks
    .replace(/\\n/g, '\n')
    // Replace \t with tabs
    .replace(/\\t/g, '\t')
    // Replace \" with quotes
    .replace(/\\"/g, '"')
    // Replace \' with single quotes
    .replace(/\\'/g, "'")
    // Replace \\ with single backslash
    .replace(/\\\\/g, '\\');

  return formatted;
};

/**
 * Parse chatbot response and extract answer field if exists
 * @param {Object} response - API response object
 * @returns {string} - Formatted answer text
 */
export const parseChatbotResponse = (response) => {
  try {
    // If response has answer field
    if (response && response.answer) {
      return formatChatbotResponse(response.answer);
    }
    
    // If response is a string
    if (typeof response === 'string') {
      return formatChatbotResponse(response);
    }

    // If response has data.answer
    if (response && response.data && response.data.answer) {
      return formatChatbotResponse(response.data.answer);
    }

    return 'No response received';
  } catch (error) {
    console.error('Error parsing chatbot response:', error);
    return 'Error processing response';
  }
};

/**
 * Split text into paragraphs for better rendering
 * @param {string} text - Formatted text
 * @returns {Array<string>} - Array of paragraphs
 */
export const splitIntoParagraphs = (text) => {
  if (!text) return [];
  
  return text
    .split('\n')
    .map(line => line.trim())
    .filter(line => line.length > 0);
};

/**
 * Format text with markdown-like syntax
 * @param {string} text - Text to format
 * @returns {string} - HTML formatted text
 */
export const formatMarkdown = (text) => {
  if (!text) return '';

  let formatted = text
    // Bold text **text**
    .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
    // Italic text *text*
    .replace(/\*(.*?)\*/g, '<em>$1</em>')
    // Code blocks `code`
    .replace(/`(.*?)`/g, '<code>$1</code>');

  return formatted;
};

/**
 * Clean and format chatbot message for display
 * @param {string} text - Raw message text
 * @param {boolean} enableMarkdown - Whether to process markdown
 * @returns {string} - Clean formatted text
 */
export const cleanChatbotMessage = (text, enableMarkdown = false) => {
  if (!text) return '';

  let cleaned = formatChatbotResponse(text);
  
  if (enableMarkdown) {
    cleaned = formatMarkdown(cleaned);
  }

  return cleaned;
};
