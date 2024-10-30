import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import BookList from './components/BookList';
import MyReservations from './components/MyReservations';

function App() {
  return (
    <Router>
      <div className="min-h-screen bg-gray-100 p-6">
        <header className="mb-6">
          <nav className="flex space-x-4">
            <Link
              to="/"
              className="inline-block px-4 py-2 text-white bg-blue-500 rounded hover:bg-blue-600 transition-colors"
            >
              Book List
            </Link>
            <Link
              to="/reservations"
              className="inline-block px-4 py-2 text-white bg-blue-500 rounded hover:bg-blue-600 transition-colors"
            >
              My Reservations
            </Link>
          </nav>
        </header>
        <main>
          <Routes>
            <Route path="/" element={<BookList />} />
            <Route path="/reservations" element={<MyReservations />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
