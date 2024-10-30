import React from 'react';

const ConfirmationDialog = ({ message, onConfirm, onCancel }) => {
    return (
        <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
            <div className="bg-white rounded p-6 shadow-lg">
                <h2 className="text-xl font-semibold mb-4">Confirmation</h2>
                <p>{message}</p>
                <div className="flex justify-between mt-6">
                    <button onClick={onConfirm} className="bg-blue-500 text-white rounded px-4 py-2">Confirm</button>
                    <button onClick={onCancel} className="bg-gray-300 rounded px-4 py-2">Cancel</button>
                </div>
            </div>
        </div>
    );
};

export default ConfirmationDialog;
