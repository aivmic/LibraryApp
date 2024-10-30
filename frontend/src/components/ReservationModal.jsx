// ReservationModal.jsx
import React, { useState } from 'react';

function ReservationModal({ book, onClose, onSubmit }) {
    const [bookType, setBookType] = useState("Book");
    const [pickup, setPickup] = useState(false);
    const [days, setDays] = useState(1);

    const handleSubmit = () => {
        const reservationDetails = {
            bookId: book.id,
            days,
            quickPickup: pickup,
            bookType,
        };
        onSubmit(reservationDetails);
    };

    return (
        <div className="fixed inset-0 flex items-center justify-center bg-gray-800 bg-opacity-50">
            <div className="bg-white p-6 rounded shadow-lg max-w-sm w-full">
                <h2 className="text-lg font-semibold mb-4">Reserve {book.name}</h2>
                <div className="mb-4">
                    <label className="block">Book Type:</label>
                    <select value={bookType} onChange={(e) => setBookType(e.target.value)} className="border p-2 rounded w-full">
                        <option value="Book">Book</option>
                        <option value="Audiobook">Audiobook</option>
                    </select>
                </div>
                <div className="mb-4">
                    <label className="block">Quick Pickup:</label>
                    <input type="checkbox" checked={pickup} onChange={() => setPickup(!pickup)} />
                </div>
                <div className="mb-4">
                    <label className="block">Days to Reserve:</label>
                    <input
                        type="number"
                        min="1"
                        value={days}
                        onChange={(e) => setDays(e.target.value)}
                        className="border p-2 rounded w-full"
                    />
                </div>
                <button onClick={handleSubmit} className="bg-blue-500 text-white py-2 px-4 rounded hover:bg-blue-600 w-full">Confirm</button>
                <button onClick={onClose} className="mt-2 text-gray-600 w-full">Cancel</button>
            </div>
        </div>
    );
}

export default ReservationModal;
