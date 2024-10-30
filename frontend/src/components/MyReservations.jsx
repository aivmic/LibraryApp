// MyReservations.jsx
import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './index.css';

const BASE_URL = process.env.REACT_APP_API_URL;

function MyReservations() {
    const [reservations, setReservations] = useState([]);

    const fetchReservations = async () => {
        try {
            const response = await axios.get(`${BASE_URL}/reservations`);
            setReservations(response.data);
            console.log(response.data);
        } catch (error) {
            console.error("Error fetching reservations:", error);
        }
    };

    useEffect(() => {
        fetchReservations();
    }, []);

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-4">My Reservations</h1>
            <div className="grid grid-cols-1 gap-4">
                {reservations.map((reservation) => (
                    <div key={reservation.id} className="border rounded p-4 shadow-md">
                        <h2 className="text-xl font-semibold">Book ID: {reservation.bookId}</h2>
                        <p>Days Reserved: {reservation.days}</p>
                        <p>Quick Pickup: {reservation.quickPickUp ? 'Yes' : 'No'}</p>
                        <p>Total Cost: ${reservation.totalCost}</p>
                        <p>Date Reserved: {new Date(reservation.reservationDate).toLocaleDateString()}</p>
                        <p>Return Date: {new Date(reservation.returnDate).toLocaleDateString()}</p>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default MyReservations;
