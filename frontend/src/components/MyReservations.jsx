import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './index.css';

const BASE_URL = process.env.REACT_APP_API_URL;

function MyReservations() {
    const [reservations, setReservations] = useState([]);
    const [bookDetails, setBookDetails] = useState({});
    const [confirmDelete, setConfirmDelete] = useState({ show: false, reservationId: null });

    const fetchReservations = async () => {
        try {
            const response = await axios.get(`${BASE_URL}/reservations`);
            setReservations(response.data);
            return response.data;
        } catch (error) {
            console.error("Error fetching reservations:", error);
        }
    };

    const fetchBookDetails = async (bookIds) => {
        try {
            const bookRequests = bookIds.map(id => axios.get(`${BASE_URL}/books/${id}`));
            const responses = await Promise.all(bookRequests);
            const details = {};
            responses.forEach(response => {
                const book = response.data;
                details[book.id] = { name: book.name, picture: book.picture };
            });
            setBookDetails(details);
        } catch (error) {
            console.error("Error fetching book details:", error);
        }
    };

    useEffect(() => {
        const loadReservations = async () => {
            const reservationsData = await fetchReservations();
            if (reservationsData) {
                const bookIds = reservationsData.map(reservation => reservation.bookId);
                await fetchBookDetails(bookIds);
            }
        };

        loadReservations();
    }, []);

    const handleDeleteClick = (id) => {
        setConfirmDelete({ show: true, reservationId: id });
    };

    const confirmDeleteReservation = async () => {
        try {
            await axios.delete(`${BASE_URL}/reservations/${confirmDelete.reservationId}`);
            setReservations(prev => prev.filter(reservation => reservation.id !== confirmDelete.reservationId));
            setConfirmDelete({ show: false, reservationId: null });
        } catch (error) {
            console.error("Error deleting reservation:", error);
        }
    };

    const cancelDelete = () => {
        setConfirmDelete({ show: false, reservationId: null });
    };

    return (
        <div className="p-6 bg-gray-100 min-h-screen">
            <h1 className="text-3xl font-bold text-center mb-8 text-grey-800">My Reservations</h1>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                {reservations.map((reservation) => {
                    const book = bookDetails[reservation.bookId];
                    return (
                        <div key={reservation.id} className="bg-white border border-gray-200 rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-300">
                            {book ? (
                                <>
                                    <h2 className="text-xl font-semibold text-center">{book.name}</h2>
                                    <img
                                        src={`${BASE_URL.replace('/api', '')}${book.picture}`}
                                        alt={`Cover of ${book.name}`}
                                        className="w-full h-85 object-cover"
                                    />
                                </>
                            ) : (
                                <p>Loading book details...</p>
                            )}
                            <p>Days Reserved: {reservation.days}</p>
                            <p>Quick Pickup: {reservation.quickPickUp ? 'Yes' : 'No'}</p>
                            <p>Total Cost: €{reservation.totalCost}</p>
                            <p>Date Reserved: {new Date(reservation.reservationDate).toLocaleDateString()}</p>
                            <p>Return Date: {new Date(reservation.returnDate).toLocaleDateString()}</p>
                            <button
                                onClick={() => handleDeleteClick(reservation.id)}
                                className="mt-4 w-full bg-red-500 text-white py-2 px-4 rounded-lg hover:bg-red-600 transition-colors duration-300 text-center"
                            >
                                Delete Reservation
                            </button>
                        </div>
                    );
                })}
            </div>

            {confirmDelete.show && (
                <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50">
                    <div className="bg-white rounded p-6 shadow-lg">
                        <h2 className="text-xl font-semibold mb-4">Confirm Deletion</h2>
                        <p>Are you sure you want to delete this reservation?</p>
                        <div className="flex justify-between mt-6">
                            <button onClick={confirmDeleteReservation} className="bg-red-500 text-white rounded px-4 py-2">Yes, Delete</button>
                            <button onClick={cancelDelete} className="bg-gray-300 rounded px-4 py-2">Cancel</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default MyReservations;
