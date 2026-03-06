-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               5.7.24 - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             10.2.0.5599
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping data for table laybsisu_dbs.acession_tbl: ~10 rows (approximately)
/*!40000 ALTER TABLE `acession_tbl` DISABLE KEYS */;
INSERT INTO `acession_tbl` (`ID`, `TransactionNo`, `AccessionID`, `ISBN`, `Barcode`, `BookTitle`, `Shelf`, `SupplierName`, `Donor`, `Status`) VALUES
	(1, 'T-00001', '51036', NULL, '3860050162364', 'Ibong Adarna', '1', 'ABC Supplier', '', 'Available'),
	(2, 'T-00001', '97237', NULL, '3860050162364', 'Ibong Adarna', '1', 'ABC Supplier', '', 'Available'),
	(3, 'T-00001', '27309', NULL, '3860050162364', 'Ibong Adarna', '1', 'ABC Supplier', '', 'Available'),
	(4, 'T-00001', '34606', NULL, '3860050162364', 'Ibong Adarna', '1', 'ABC Supplier', '', 'Available'),
	(5, 'T-00001', '82498', NULL, '3860050162364', 'Ibong Adarna', '1', 'ABC Supplier', '', 'Available'),
	(6, 'T-00002', '53816', NULL, '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available'),
	(7, 'T-00002', '24111', NULL, '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available'),
	(8, 'T-00002', '32467', NULL, '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available'),
	(9, 'T-00002', '71186', NULL, '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available'),
	(10, 'T-00002', '82253', NULL, '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available');
/*!40000 ALTER TABLE `acession_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.acquisition_tbl: ~2 rows (approximately)
/*!40000 ALTER TABLE `acquisition_tbl` DISABLE KEYS */;
INSERT INTO `acquisition_tbl` (`ID`, `ISBN`, `Barcode`, `BookTitle`, `SupplierName`, `Donor`, `Quantity`, `BookPrice`, `TotalCost`, `TransactionNo`, `DateAcquired`) VALUES
	(1, '', '3860050162364', 'Ibong Adarna', 'ABC Supplier', '', '5', '123.00', '615.00', 'T-00001', '2026-03-03'),
	(2, '', '3831647888411', 'Javascript (For Beginner)', 'ABC Supplier', '', '5', '123', '615', 'T-00002', '2026-03-03');
/*!40000 ALTER TABLE `acquisition_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.audit_trail_tbl: ~141 rows (approximately)
/*!40000 ALTER TABLE `audit_trail_tbl` DISABLE KEYS */;
INSERT INTO `audit_trail_tbl` (`ID`, `Role`, `Email`, `ActionType`, `FormName`, `Description`, `DateTime`) VALUES
	(1, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/01/26-10:28 pm'),
	(2, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'USER STAFF FORM', 'Added new staff: Lindungan, Nazzer (Assistant Librarian)', '03/01/26-10:31 pm'),
	(3, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 192.168.100.109.', '03/01/26-10:32 PM'),
	(4, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'AUTHOR FORM', 'Added new Author: nazz', '03/01/26-10:33 PM'),
	(5, 'Assistant Librarian', 'xzzu09@gmail.com', 'UPDATE', 'AUTHOR FORM', 'Updated Author Name. [Change: nazz -> nazzuu]', '03/01/26-10:33 PM'),
	(6, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'AUTHOR FORM', 'Added new Author: nazzu', '03/01/26-10:33 PM'),
	(7, 'Assistant Librarian', 'xzzu09@gmail.com', 'UPDATE', 'AUTHOR FORM', 'Updated Author Name. [Change: nazzu -> jade]', '03/01/26-10:34 PM'),
	(8, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'AUTHOR FORM', 'Added new Author: monge', '03/01/26-10:34 PM'),
	(9, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'AUTHOR FORM', 'Updated Author Name. [Change: jade -> jadesu]', '03/01/26-10:34 pm'),
	(10, 'Assistant Librarian', 'xzzu09@gmail.com', 'UPDATE', 'AUTHOR FORM', 'Updated Author Name. [Change: jadesu -> jadesus]', '03/01/26-10:35 PM'),
	(11, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'AUTHOR FORM', 'Updated Author Name. [Change: jadesus -> jadesussssss]', '03/01/26-10:35 pm'),
	(12, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'AUTHOR FORM', 'Added new Author: angelo', '03/01/26-10:36 PM'),
	(13, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'GENRE FORM', 'Added new Genre: Horror', '03/01/26-10:36 PM'),
	(14, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'LANGUAGE FORM', 'Added new language: English', '03/01/26-10:37 PM'),
	(15, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'LANGUAGE FORM', 'Added new language: Spanish', '03/01/26-10:37 pm'),
	(16, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'USER STAFF FORM', 'Updated staff ID 1: Lindungan, Nazzer (Staff) [Change: Nazzer|Lindungan|N/A|assistant|assistant@123|xzzu09@gmail.com|09242357456|Taguig|Male|Assistant Librarian -> Nazzer|Lindungan|N/A|assistant|assistant@123|xzzu09@gmail.com|09242357456|Taguig|Male|Staff]', '03/01/26-10:39 pm'),
	(17, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/01/26-10:40 PM'),
	(18, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/01/26-10:41 pm'),
	(19, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/01/26-10:47 pm'),
	(20, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'USER STAFF FORM', 'Updated staff ID 1: Lindungan, Nazzer (Assistant Librarian) [Change: Nazzer|Lindungan|N/A|assistant|assistant@123|xzzu09@gmail.com|09242357456|Taguig|Male|Staff -> Nazzer|Lindungan|N/A|assistant|assistant@123|xzzu09@gmail.com|09242357456|Taguig|Male|Assistant Librarian]', '03/01/26-10:47 pm'),
	(21, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 192.168.100.109.', '03/01/26-10:49 PM'),
	(22, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/01/26-10:49 pm'),
	(23, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/01/26-10:51 PM'),
	(24, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 192.168.100.109.', '03/01/26-10:53 PM'),
	(25, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'PUBLISHER FORM', 'Added new publisher: Kean Abesa', '03/01/26-10:54 PM'),
	(26, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/01/26-10:54 pm'),
	(27, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'SUPPLIER FORM', 'Added new supplier: ABC Supplier', '03/01/26-10:55 pm'),
	(28, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'BOOK FORM', 'Added new Book: Ibong Adarna', '03/01/26-10:55 PM'),
	(29, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'DEPARTMENT FORM', 'Added new department: Junior High School', '03/01/26-10:56 pm'),
	(30, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'DEPARTMENT FORM', 'Added new department: Senior High School', '03/01/26-10:56 pm'),
	(31, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'DEPARTMENT FORM', 'Added new department: Elementary', '03/01/26-10:56 pm'),
	(32, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'GRADE FORM', 'Added new grade level: 7', '03/01/26-10:56 pm'),
	(33, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'GRADE FORM', 'Added new grade level: 8', '03/01/26-10:56 pm'),
	(34, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'GRADE FORM', 'Added new grade level: 9', '03/01/26-10:56 pm'),
	(35, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'GRADE FORM', 'Added new grade level: 10', '03/01/26-10:57 PM'),
	(36, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'GRADE FORM', 'Added new grade level: 11', '03/01/26-10:57 PM'),
	(37, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'GRADE FORM', 'Added new grade level: 12', '03/01/26-10:57 PM'),
	(38, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'SECTION FORM', 'Added new section/strand: Department: Junior High School, Grade Level: 7, Section: Makahiya', '03/01/26-10:57 pm'),
	(39, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'STRAND FORM', 'Added new strand: HUMSS (Humanities and Social Sciences)', '03/01/26-10:58 pm'),
	(40, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'STRAND FORM', 'Added new strand: STEM (Science, Technology, Engineering, and Mathematics)', '03/01/26-10:58 pm'),
	(41, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'STRAND FORM', 'Added new strand: GAS (General Academic Strand)', '03/01/26-10:58 pm'),
	(42, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'SECTION FORM', 'Added new section/strand: Department: Senior High School, Grade Level: 11, Strand: HUMSS', '03/01/26-10:58 pm'),
	(43, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'SECTION FORM', 'Added new section/strand: Department: Senior High School, Grade Level: 12, Strand: STEM', '03/01/26-10:58 pm'),
	(44, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/01/26-11:00 PM'),
	(45, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'USER STAFF FORM', 'Updated staff ID 1: Lindungan, Nazzer (Assistant Librarian) [Change: Nazzer|Lindungan|N/A|assistant|assistant@123|xzzu09@gmail.com|09242357456|Taguig|Male|Assistant Librarian -> Nazzer|Lindungan|N/A|assistant|assistant@123|xzzu09@gmail.com|09242357456|Manila|Male|Assistant Librarian]', '03/01/26-11:01 pm'),
	(46, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'SHELF FORM', 'Added new shelf: 1', '03/01/26-11:01 pm'),
	(47, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'SHELF FORM', 'Added new shelf: 2', '03/01/26-11:01 pm'),
	(48, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'SHELF FORM', 'Added new shelf: 3', '03/01/26-11:01 pm'),
	(49, 'Librarian', 'nazzrodin01@gmail.com', 'INSERT', 'ACQUISITION FORM', 'Added New Acquisition Record for Book: Ibong Adarna (ISBN: , TransNo: T-00001)', '03/01/26-11:02 pm'),
	(50, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: 91676, 44531, 26365, 45074, 33093, 60069, 63655, 22520, 58418, 70635. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: 91676, 44531, 26365, 45074, 33093, 60069, 63655, 22520, 58418, 70635]', '03/01/26-11:02 pm'),
	(51, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BORROWER FORM', 'Added new Student: Lindungan, Nazzrodin', '03/01/26-11:03 pm'),
	(52, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/01/26-11:05 pm'),
	(53, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/02/26-5:23 am'),
	(54, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 192.168.100.109.', '03/02/26-5:24 AM'),
	(55, 'Assistant Librarian', 'xzzu09@gmail.com', 'UPDATE', 'USER STAFF FORM', 'Updated staff ID 1: Lindungan, Nazzer (Assistant Librarian) [Change: Nazzer|Lindungan|N/A|assistant|assistant@123|xzzu09@gmail.com|09242357456|Manila|Male|Assistant Librarian -> Nazzer|Lindungan|S|assistant|assistant@123|xzzu09@gmail.com|09242357456|Manila|Male|Assistant Librarian]', '03/02/26-5:26 AM'),
	(56, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'USER STAFF FORM', 'Added new staff: Cejo, Earl John (Staff)', '03/02/26-5:36 am'),
	(57, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/02/26-5:37 AM'),
	(58, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 192.168.100.109.', '03/02/26-5:38 AM'),
	(59, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/02/26-5:39 AM'),
	(60, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/02/26-5:40 am'),
	(61, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/02/26-5:49 am'),
	(62, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 192.168.100.109.', '03/02/26-5:49 AM'),
	(63, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/02/26-5:51 AM'),
	(64, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/02/26-5:52 am'),
	(65, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/02/26-2:58 pm'),
	(66, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/02/26-2:58 pm'),
	(67, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/02/26-3:11 pm'),
	(68, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/02/26-3:11 pm'),
	(69, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-8:23 pm'),
	(70, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'AUTHOR FORM', 'Updated Author Name. [Change: nazzuu -> zzz]', '03/03/26-8:23 pm'),
	(71, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'GENRE FORM', 'Added new Genre: Comedy', '03/03/26-8:23 pm'),
	(72, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'GENRE FORM', 'Updated Genre Name from \'Comedy\' to \'Comedyy\'. [Change: Comedy -> Comedyy]', '03/03/26-8:24 pm'),
	(73, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BOOK FORM', 'Added new Book: Ibong Adarna', '03/03/26-8:25 pm'),
	(74, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'GENRE FORM', 'Updated Genre Name from \'Comedyy\' to \'Comedy\'. [Change: Comedyy -> Comedy]', '03/03/26-8:25 pm'),
	(75, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'BOOK FORM', 'Updated Book Title from \'Ibong Adarna\' to \'Ibong Adarna\' (ID: 1) [Change: Ibong Adarna -> Ibong Adarna]', '03/03/26-8:25 pm'),
	(76, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BOOK FORM', 'Added new Book: Ibong Adarna', '03/03/26-8:26 pm'),
	(77, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-8:26 pm'),
	(78, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-8:31 pm'),
	(79, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-8:32 pm'),
	(80, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-8:35 pm'),
	(81, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BOOK FORM', 'Added new Book: Ibong Adarna', '03/03/26-8:39 pm'),
	(82, 'Librarian', 'nazzrodin01@gmail.com', 'INSERT', 'ACQUISITION FORM', 'Added New Acquisition Record for Book: Ibong Adarna (ISBN: , TransNo: T-00001)', '03/03/26-8:40 pm'),
	(83, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: 34326, 20059, 74750, 20285, 46716. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: 34326, 20059, 74750, 20285, 46716]', '03/03/26-8:40 pm'),
	(84, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BORROWING FORM', 'Book \'Ibong Adarna\' (AccessionID: 20059) added to transaction 260303204141 for borrower Myra Bayot B. [Change: N/A -> Borrower: Myra Bayot B (01234567), Book: Ibong Adarna, AccID: 20059]', '03/03/26-8:41 pm'),
	(85, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BOOK RETURN', 'Returned 1 book(s) for transaction 260303204141. Status: Damaged. [Change: Borrower: Myra Bayot B -> Returned Books: Ibong Adarna]', '03/03/26-8:43 pm'),
	(86, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'PENALTY MANAGEMENT', 'Added new penalty: Damaged Book - Major', '03/03/26-8:44 pm'),
	(87, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PENALTY PAYMENT', 'Transaction 260303204141 marked as PENALIZED in penalty_tbl and returning_tbl. [Change: Status: NOT PENALIZED | Fee: Calculated Fee: 400.00 -> Status: PENALIZED | Paid Amount: 400.00]', '03/03/26-8:45 pm'),
	(88, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'PENALTY MANAGEMENT', 'Added new penalty: Overdue', '03/03/26-8:46 pm'),
	(89, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'PENALTY MANAGEMENT', 'Added new penalty: Lost Book', '03/03/26-8:46 pm'),
	(90, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'PENALTY MANAGEMENT', 'Added new penalty: Damaged Book - Minor', '03/03/26-8:46 pm'),
	(91, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'PENALTY MANAGEMENT', 'Added new penalty: Damaged Book - Irreparable', '03/03/26-8:46 pm'),
	(92, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: COMPLETED. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: COMPLETED]', '03/03/26-8:47 pm'),
	(93, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-8:52 pm'),
	(94, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-9:01 pm'),
	(95, 'Librarian', 'nazzrodin01@gmail.com', 'DELETE ALL', 'ACCESSION FORM', 'Permanently deleted ALL accession records (6 copies, 1 transactions). [Change: ALL ACCESSION RECORDS -> N/A (All deleted)]', '03/03/26-9:01 pm'),
	(96, 'Librarian', 'nazzrodin01@gmail.com', 'INSERT', 'ACQUISITION FORM', 'Added New Acquisition Record for Book: Ibong Adarna (ISBN: , TransNo: T-00001)', '03/03/26-9:04 pm'),
	(97, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: 27030, 82722, 17631, 62785, 42837. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: 27030, 82722, 17631, 62785, 42837]', '03/03/26-9:04 pm'),
	(98, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: COMPLETED. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: COMPLETED]', '03/03/26-9:04 pm'),
	(99, 'Librarian', 'nazzrodin01@gmail.com', 'DELETE ALL', 'ACCESSION FORM', 'Permanently deleted ALL accession records (6 copies, 1 transactions). [Change: ALL ACCESSION RECORDS -> N/A (All deleted)]', '03/03/26-9:04 pm'),
	(100, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-9:04 pm'),
	(101, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-9:05 pm'),
	(102, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: 26974, 44307, 94029, 19133, 74231. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: 26974, 44307, 94029, 19133, 74231]', '03/03/26-9:05 pm'),
	(103, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: COMPLETED. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: COMPLETED]', '03/03/26-9:06 pm'),
	(104, 'Librarian', 'nazzrodin01@gmail.com', 'DELETE ALL', 'ACCESSION FORM', 'Permanently deleted ALL accession records (6 copies, 1 transactions). [Change: ALL ACCESSION RECORDS -> N/A (All deleted)]', '03/03/26-9:06 pm'),
	(105, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-9:06 pm'),
	(106, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-9:07 pm'),
	(107, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: 21141, 17729, 61648, 37720, 34325. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: 21141, 17729, 61648, 37720, 34325]', '03/03/26-9:08 pm'),
	(108, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: COMPLETED. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: COMPLETED]', '03/03/26-9:08 pm'),
	(109, 'Librarian', 'nazzrodin01@gmail.com', 'DELETE ALL', 'ACCESSION FORM', 'Permanently deleted ALL accession records (6 copies, 1 transactions). [Change: ALL ACCESSION RECORDS -> N/A (All deleted)]', '03/03/26-9:08 pm'),
	(110, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-9:08 pm'),
	(111, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-9:10 pm'),
	(112, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: 64806, 34946, 26924, 33052, 13152. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: 64806, 34946, 26924, 33052, 13152]', '03/03/26-9:11 pm'),
	(113, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-9:11 pm'),
	(114, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-9:13 pm'),
	(115, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BORROWING FORM', 'Book \'Ibong Adarna\' (AccessionID: 13152) added to transaction 260303211328 for borrower Myra Bayot B. [Change: N/A -> Borrower: Myra Bayot B (01234567), Book: Ibong Adarna, AccID: 13152]', '03/03/26-9:13 pm'),
	(116, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BOOK RETURN', 'Returned 1 book(s) for transaction 260303211328. Status: Damaged. [Change: Borrower: Myra Bayot B -> Returned Books: Ibong Adarna]', '03/03/26-9:14 pm'),
	(117, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Damaged (Major), Old Accession: Damaged -> New Status: Lost, New Accession: Lost]', '03/03/26-9:15 pm'),
	(118, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PENALTY PAYMENT', 'Transaction 260303211328 marked as PENALIZED in penalty_tbl and returning_tbl. [Change: Status: NOT PENALIZED | Fee: Calculated Fee: 123.00 -> Status: PENALIZED | Paid Amount: 123.00]', '03/03/26-9:16 pm'),
	(119, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: 73501, 29129, 68967. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: 73501, 29129, 68967]', '03/03/26-9:29 pm'),
	(120, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BORROWING FORM', 'Book \'Ibong Adarna\' (AccessionID: 73501) added to transaction 260303211328 for borrower Myra Bayot B. [Change: N/A -> Borrower: Myra Bayot B (01234567), Book: Ibong Adarna, AccID: 73501]', '03/03/26-9:29 pm'),
	(121, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BOOK RETURN', 'Returned 1 book(s) for transaction 260303211328. Status: Damaged. [Change: Borrower: Myra Bayot B -> Returned Books: Ibong Adarna]', '03/03/26-9:30 pm'),
	(122, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'ACQUISITION FORM', 'Updated Acquisition Record for Book: Ibong Adarna (ID: 1)', '03/03/26-9:32 pm'),
	(123, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: 51036, 97237, 27309, 34606, 82498. Title: Ibong Adarna [Change: N/A -> Status: Available, IDs: 51036, 97237, 27309, 34606, 82498]', '03/03/26-9:32 pm'),
	(124, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BORROWING FORM', 'Book \'Ibong Adarna\' (AccessionID: 34606) added to transaction 260303211328 for borrower Myra Bayot B. [Change: N/A -> Borrower: Myra Bayot B (01234567), Book: Ibong Adarna, AccID: 34606]', '03/03/26-9:32 pm'),
	(125, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BOOK RETURN', 'Returned 1 book(s) for transaction 260303211328. Status: Damaged. [Change: Borrower: Myra Bayot B -> Returned Books: Ibong Adarna]', '03/03/26-9:33 pm'),
	(126, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Damaged (Major), Old Accession: Damaged -> New Status: Lost, New Accession: Lost]', '03/03/26-9:33 pm'),
	(127, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Lost, Old Accession: Lost -> New Status: Damaged (Major), New Accession: Damaged]', '03/03/26-9:34 pm'),
	(128, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Damaged (Major), Old Accession: Damaged -> New Status: Damaged (Minor), New Accession: Available]', '03/03/26-9:34 pm'),
	(129, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Damaged (Minor), Old Accession: Available -> New Status: Normal, New Accession: Available]', '03/03/26-9:35 pm'),
	(130, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Normal, Old Accession: Available -> New Status: Damaged (Major), New Accession: Damaged]', '03/03/26-9:36 pm'),
	(131, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Damaged (Major), Old Accession: Damaged -> New Status: Damaged (Minor), New Accession: Available]', '03/03/26-9:36 pm'),
	(132, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-9:37 pm'),
	(133, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-9:38 pm'),
	(134, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-9:39 pm'),
	(135, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-9:48 pm'),
	(136, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Damaged (Minor), Old Accession: Available -> New Status: Damaged (Major), New Accession: Damaged]', '03/03/26-9:48 pm'),
	(137, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Damaged (Major), Old Accession: Damaged -> New Status: Normal, New Accession: Available]', '03/03/26-9:49 pm'),
	(138, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Normal, Old Accession: Available -> New Status: Lost, New Accession: Lost]', '03/03/26-9:49 pm'),
	(139, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'RETURN FORM', 'Edited status of book \'Ibong Adarna\' in transaction 260303211328. [Change: Book: Ibong Adarna, Old Status: Lost, Old Accession: Lost -> New Status: Normal, New Accession: Available]', '03/03/26-9:49 pm'),
	(140, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-9:49 pm'),
	(141, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/03/26-9:52 pm'),
	(142, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BOOK FORM', 'Added new Book: Javascript (For Beginner)', '03/03/26-9:53 pm'),
	(143, 'Librarian', 'nazzrodin01@gmail.com', 'INSERT', 'ACQUISITION FORM', 'Added New Acquisition Record for Book: Javascript (For Beginner) (ISBN: , TransNo: T-00002)', '03/03/26-9:53 pm'),
	(144, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'ACCESSION FORM', 'Added new Accession records: 53816, 24111, 32467, 71186, 82253. Title: Javascript (For Beginner) [Change: N/A -> Status: Available, IDs: 53816, 24111, 32467, 71186, 82253]', '03/03/26-9:54 pm'),
	(145, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/03/26-9:54 pm');
/*!40000 ALTER TABLE `audit_trail_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.author_tbl: ~4 rows (approximately)
/*!40000 ALTER TABLE `author_tbl` DISABLE KEYS */;
INSERT INTO `author_tbl` (`ID`, `AuthorName`) VALUES
	(1, 'zzz'),
	(2, 'jadesussssss'),
	(3, 'monge'),
	(4, 'angelo');
/*!40000 ALTER TABLE `author_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.available_tbl: ~10 rows (approximately)
/*!40000 ALTER TABLE `available_tbl` DISABLE KEYS */;
INSERT INTO `available_tbl` (`ID`, `ISBN`, `Barcode`, `AccessionID`, `BookTitle`, `Shelf`, `Status`) VALUES
	(43, NULL, '3860050162364', '51036', 'Ibong Adarna', '1', 'Available'),
	(44, NULL, '3860050162364', '97237', 'Ibong Adarna', '1', 'Available'),
	(45, NULL, '3860050162364', '27309', 'Ibong Adarna', '1', 'Available'),
	(46, NULL, '3860050162364', '34606', 'Ibong Adarna', '1', 'Available'),
	(47, NULL, '3860050162364', '82498', 'Ibong Adarna', '1', 'Available'),
	(48, NULL, '3831647888411', '53816', 'Javascript (For Beginner)', '2', 'Available'),
	(49, NULL, '3831647888411', '24111', 'Javascript (For Beginner)', '2', 'Available'),
	(50, NULL, '3831647888411', '32467', 'Javascript (For Beginner)', '2', 'Available'),
	(51, NULL, '3831647888411', '71186', 'Javascript (For Beginner)', '2', 'Available'),
	(52, NULL, '3831647888411', '82253', 'Javascript (For Beginner)', '2', 'Available');
/*!40000 ALTER TABLE `available_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.book_tbl: ~1 rows (approximately)
/*!40000 ALTER TABLE `book_tbl` DISABLE KEYS */;
INSERT INTO `book_tbl` (`ID`, `Barcode`, `ISBN`, `BookTitle`, `Author`, `Genre`, `Publisher`, `Language`, `YearPublished`) VALUES
	(4, '3860050162364', NULL, 'Ibong Adarna', 'jadesussssss', 'Comedy', 'Kean Abesa', 'English', '2002'),
	(5, '3831647888411', NULL, 'Javascript (For Beginner)', 'monge', 'Comedy', 'Kean Abesa', 'English', '2012');
/*!40000 ALTER TABLE `book_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.borrowedview_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `borrowedview_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `borrowedview_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.borroweredit_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `borroweredit_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `borroweredit_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.borrower_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `borrower_tbl` DISABLE KEYS */;
INSERT INTO `borrower_tbl` (`ID`, `Borrower`, `FirstName`, `LastName`, `MiddleInitial`, `LRN`, `EmployeeNo`, `ContactNumber`, `Department`, `Grade`, `Section`, `Strand`) VALUES
	(6, 'Teacher', 'Myra', 'Bayot', 'B', NULL, '01234567', '09936545252', 'Junior High School', '', '', ''),
	(7, 'Student', 'Nazzrodin', 'Lindungan', 'N/A', '136875100443', NULL, '09265656564', 'Senior High School', '12', '', 'STEM');
/*!40000 ALTER TABLE `borrower_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.borrowinghistory_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `borrowinghistory_tbl` DISABLE KEYS */;
INSERT INTO `borrowinghistory_tbl` (`ID`, `Borrower`, `ISBN`, `Barcode`, `AccessionID`, `BookTitle`, `Shelf`, `LRN`, `EmployeeNo`, `Name`, `BorrowedDate`, `DueDate`, `TransactionReceipt`, `Status`) VALUES
	(1, 'Teacher', '', '3860050162364', '34606', 'Ibong Adarna', '1', NULL, '01234567', 'Myra Bayot B', '2026-03-03 00:00:00', '2026-03-10 00:00:00', '260303211328', 'Granted');
/*!40000 ALTER TABLE `borrowinghistory_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.borrowing_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `borrowing_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `borrowing_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.category_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `category_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `category_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.confimation_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `confimation_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `confimation_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.damagedview_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `damagedview_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `damagedview_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.department_tbl: ~2 rows (approximately)
/*!40000 ALTER TABLE `department_tbl` DISABLE KEYS */;
INSERT INTO `department_tbl` (`ID`, `Department`) VALUES
	(1, 'Junior High School'),
	(2, 'Senior High School'),
	(3, 'Elementary');
/*!40000 ALTER TABLE `department_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.email_config: ~0 rows (approximately)
/*!40000 ALTER TABLE `email_config` DISABLE KEYS */;
INSERT INTO `email_config` (`Email`, `AppPassword`) VALUES
	('mdalms.bsit01@gmail.com', 'sdsx acwh twht jnba');
/*!40000 ALTER TABLE `email_config` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.genre_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `genre_tbl` DISABLE KEYS */;
INSERT INTO `genre_tbl` (`ID`, `Genre`) VALUES
	(1, 'Horror'),
	(2, 'Comedy');
/*!40000 ALTER TABLE `genre_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.grade_tbl: ~6 rows (approximately)
/*!40000 ALTER TABLE `grade_tbl` DISABLE KEYS */;
INSERT INTO `grade_tbl` (`ID`, `Grade`) VALUES
	(1, '7'),
	(2, '8'),
	(3, '9'),
	(4, '10'),
	(5, '11'),
	(6, '12');
/*!40000 ALTER TABLE `grade_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.language_tbl: ~2 rows (approximately)
/*!40000 ALTER TABLE `language_tbl` DISABLE KEYS */;
INSERT INTO `language_tbl` (`ID`, `Language`) VALUES
	(1, 'English'),
	(2, 'Spanish');
/*!40000 ALTER TABLE `language_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.lostview_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `lostview_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `lostview_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.oras_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `oras_tbl` DISABLE KEYS */;
INSERT INTO `oras_tbl` (`ID`, `Borrower`, `Department`, `LRN`, `EmployeeNo`, `FirstName`, `LastName`, `MiddleInitial`, `Contact`, `GradeLevel`, `Section`, `Strand`, `TimeIn`, `TimeOut`) VALUES
	(1, 'Teacher', 'Junior High School', NULL, '01234567', 'Myra', 'Bayot', 'B', '09936545252', '', '', '', '2026-03-03 20:41:43', NULL);
/*!40000 ALTER TABLE `oras_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.overdueview_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `overdueview_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `overdueview_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.penalty_management_tbl: ~4 rows (approximately)
/*!40000 ALTER TABLE `penalty_management_tbl` DISABLE KEYS */;
INSERT INTO `penalty_management_tbl` (`ID`, `PenaltyType`, `Amount`, `Description`) VALUES
	(1, 'Damaged Book - Major', '400', 'none'),
	(2, 'Overdue', '10', 'none'),
	(3, 'Lost Book', '500', 'none'),
	(4, 'Damaged Book - Minor', '20', 'none'),
	(5, 'Damaged Book - Irreparable', '700', 'none');
/*!40000 ALTER TABLE `penalty_management_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.penalty_tbl: ~1 rows (approximately)
/*!40000 ALTER TABLE `penalty_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `penalty_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.printreceipt_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `printreceipt_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `printreceipt_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.publisher_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `publisher_tbl` DISABLE KEYS */;
INSERT INTO `publisher_tbl` (`ID`, `PublisherName`) VALUES
	(1, 'Kean Abesa');
/*!40000 ALTER TABLE `publisher_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.reservecopiess_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `reservecopiess_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `reservecopiess_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.reserveview_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `reserveview_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `reserveview_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.returnedview_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `returnedview_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `returnedview_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.returning_tbl: ~2 rows (approximately)
/*!40000 ALTER TABLE `returning_tbl` DISABLE KEYS */;
INSERT INTO `returning_tbl` (`ID`, `Borrower`, `LRN`, `EmployeeNo`, `FullName`, `Department`, `Grade`, `Section`, `Strand`, `ReturnedBook`, `BookTotal`, `BorrowedDate`, `DueDate`, `ReturnDate`, `TransactionReceipt`, `Status`, `BorrowerStatus`) VALUES
	(11, 'Teacher', NULL, '01234567', 'Myra Bayot B', 'Junior High School', NULL, NULL, NULL, 'Ibong Adarna', '1', '03/03/2026', '10/03/2026', '03/03/2026', '260303211328', 'Normal', 'N/A');
/*!40000 ALTER TABLE `returning_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.section_tbl: ~2 rows (approximately)
/*!40000 ALTER TABLE `section_tbl` DISABLE KEYS */;
INSERT INTO `section_tbl` (`ID`, `Department`, `GradeLevel`, `Section`, `Strand`) VALUES
	(1, 'Junior High School', '7', 'Makahiya', NULL),
	(2, 'Senior High School', '11', NULL, 'HUMSS'),
	(3, 'Senior High School', '12', NULL, 'STEM');
/*!40000 ALTER TABLE `section_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.selectbarcode_tbl: ~1 rows (approximately)
/*!40000 ALTER TABLE `selectbarcode_tbl` DISABLE KEYS */;
INSERT INTO `selectbarcode_tbl` (`ID`, `Barcode`, `BookTitle`) VALUES
	(2, '3860050162364', 'Ibong Adarna'),
	(3, '3831647888411', 'Javascript (For Beginner)');
/*!40000 ALTER TABLE `selectbarcode_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.shelf_tbl: ~2 rows (approximately)
/*!40000 ALTER TABLE `shelf_tbl` DISABLE KEYS */;
INSERT INTO `shelf_tbl` (`ID`, `Shelf`) VALUES
	(1, '1'),
	(2, '2'),
	(3, '3');
/*!40000 ALTER TABLE `shelf_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.strand_tbl: ~2 rows (approximately)
/*!40000 ALTER TABLE `strand_tbl` DISABLE KEYS */;
INSERT INTO `strand_tbl` (`ID`, `Strand`, `Description`) VALUES
	(1, 'HUMSS', 'Humanities and Social Sciences'),
	(2, 'STEM', 'Science, Technology, Engineering, and Mathematics'),
	(3, 'GAS', 'General Academic Strand');
/*!40000 ALTER TABLE `strand_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.superadmin_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `superadmin_tbl` DISABLE KEYS */;
INSERT INTO `superadmin_tbl` (`ID`, `CurrentIP`, `FirstName`, `LastName`, `MiddleName`, `ContactNumber`, `Email`, `Address`, `Username`, `Password`, `Gender`, `Role`, `is_logged_in`) VALUES
	(1, '0.0.0.0', 'Nazzrodin', 'Lindungan', 'N/A', '09109224124', 'nazzrodin01@gmail.com', 'Taguig', 'admin', 'adminadmin@123', 'Male', 'Librarian', 0);
/*!40000 ALTER TABLE `superadmin_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.supplier_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `supplier_tbl` DISABLE KEYS */;
INSERT INTO `supplier_tbl` (`ID`, `SupplierName`, `Address`, `ContactNumber`) VALUES
	(1, 'ABC Supplier', 'Manila', '09745332534');
/*!40000 ALTER TABLE `supplier_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.timeinoutrecord_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `timeinoutrecord_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `timeinoutrecord_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.totalbooksview_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `totalbooksview_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `totalbooksview_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.user_staff_tbl: ~1 rows (approximately)
/*!40000 ALTER TABLE `user_staff_tbl` DISABLE KEYS */;
INSERT INTO `user_staff_tbl` (`ID`, `CurrentIP`, `FirstName`, `LastName`, `MiddleInitial`, `Username`, `Password`, `Email`, `Address`, `ContactNumber`, `Gender`, `Role`, `is_logged_in`) VALUES
	(1, '0.0.0.0', 'Nazzer', 'Lindungan', 'S', 'assistant', 'nazzu123', 'xzzu09@gmail.com', 'Manila', '09242357456', 'Male', 'Assistant Librarian', 0),
	(2, NULL, 'Earl John', 'Cejo', 'N/A', 'staff', 'staff@12345', 'lizzjanellelerumaquino@gmail.com', 'Badingan', '09437543454', 'Male', 'Staff', NULL);
/*!40000 ALTER TABLE `user_staff_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.varkuwd_tbl: ~4 rows (approximately)
/*!40000 ALTER TABLE `varkuwd_tbl` DISABLE KEYS */;
INSERT INTO `varkuwd_tbl` (`ID`, `Barcode`) VALUES
	(1, '5597039377056'),
	(2, '0831380485892'),
	(3, '9797954434230'),
	(4, '6947108079725');
/*!40000 ALTER TABLE `varkuwd_tbl` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
