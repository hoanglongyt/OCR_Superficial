import React from 'react';

export default function BillingBox({ rawText }) {
  const lines = rawText.split('\n');

  return (
    <div className='billingbox-container'>
      {lines.map((line, index) => (
        <div key={index} className='billingbox-line'>
          {line.trim() || <span>&nbsp;</span>}
        </div>
      ))}
    </div>
  );
}