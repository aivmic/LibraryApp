// BookList.jsx
import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import ReservationModal from './ReservationModal';
import './index.css';

const BASE_URL = process.env.REACT_APP_API_URL;

function BookList() {
    const [books, setBooks] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [filterType, setFilterType] = useState('All');
    const [selectedBook, setSelectedBook] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);



    const fetchBooks = useCallback(async () => {
        try {
            const response = await axios.get(`${BASE_URL}/books`, {
                params: {
                    search: searchTerm,
                    type: filterType !== 'All' ? filterType : undefined,
                },
            });
            setBooks(response.data);
        } catch (error) {
            console.error("Error fetching books:", error);
        }
    }, [searchTerm, filterType]);

    useEffect(() => {
        fetchBooks();
    }, [fetchBooks]);



    const handleReservationClick = (book) => {
        setSelectedBook(book);
        setIsModalOpen(true);
    };

    const closeModal = () => {
        setSelectedBook(null);
        setIsModalOpen(false);
    };

    const handleReservationSubmit = async (reservationDetails) => {
        try {
            await axios.post(`${BASE_URL}/reservations`, {
                bookId: reservationDetails.bookId,
                days: reservationDetails.days,
                quickPickup: reservationDetails.quickPickup,
            });
            closeModal();
        } catch (error) {
            console.error("Error creating reservation:", error);
        }
    };

    return (
        <div className="p-6 bg-gray-100 min-h-screen">
            <h1 className="text-3xl font-bold text-center mb-8 text-grey-800">Library Books</h1>

            <div className="mb-6 flex flex-col md:flex-row justify-center items-center space-y-3 md:space-y-0 md:space-x-4">
                <input
                    type="text"
                    placeholder="Search by name, year..."
                    className="w-full md:w-1/3 px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:border-blue-400"
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                />
                <select
                    value={filterType}
                    onChange={(e) => setFilterType(e.target.value)}
                    className="w-full md:w-1/3 px-4 py-2 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:border-blue-400"
                >
                    <option value="All">All Types</option>
                    <option value="Book">Book</option>
                    <option value="Audiobook">Audiobook</option>
                </select>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                {books.map((book) => (
                    <div key={book.id} className="bg-white border border-gray-200 rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-300">
                        <img src={book.picture} alt={book.name} className="w-full h-48 object-cover" />
                        <div className="p-4">
                            <h2 className="text-xl font-semibold text-gray-800">{book.name}</h2>
                            <p className="text-gray-500">Year: {book.year}</p>
                            <button
                                onClick={() => handleReservationClick(book)}
                                className="mt-4 w-full bg-blue-500 text-white py-2 px-4 rounded-lg hover:bg-blue-600 transition-colors duration-300"
                            >
                                Reserve
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {isModalOpen && selectedBook && (
                <ReservationModal
                    book={selectedBook}
                    onClose={closeModal}
                    onSubmit={handleReservationSubmit}
                />
            )}
        </div>
    );
}

export default BookList;
