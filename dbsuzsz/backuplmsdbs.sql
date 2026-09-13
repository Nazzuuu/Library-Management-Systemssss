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

-- Dumping data for table laybsisu_dbs.acession_tbl: ~9 rows (approximately)
/*!40000 ALTER TABLE `acession_tbl` DISABLE KEYS */;
INSERT INTO `acession_tbl` (`ID`, `TransactionNo`, `AccessionID`, `ISBN`, `Barcode`, `BookTitle`, `Shelf`, `SupplierName`, `Donor`, `Status`) VALUES
	(1, 'T-00001', '51036', NULL, '3860050162364', 'Ibong Adarna', '1', 'ABC Supplier', '', 'Available'),
	(2, 'T-00001', '97237', NULL, '3860050162364', 'Ibong Adarna', '1', 'ABC Supplier', '', 'Available'),
	(3, 'T-00001', '27309', NULL, '3860050162364', 'Ibong Adarna', '1', 'ABC Supplier', '', 'Available'),
	(4, 'T-00001', '34606', NULL, '3860050162364', 'Ibong Adarna', '1', 'ABC Supplier', '', 'Borrowed'),
	(6, 'T-00002', '53816', NULL, '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available'),
	(7, 'T-00002', '24111', NULL, '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Damaged'),
	(8, 'T-00002', '32467', NULL, '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available'),
	(11, 'T-00002', '45500', '', '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available'),
	(12, 'T-00002', '11814', '', '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available'),
	(13, 'T-00002', '74881', '', '3831647888411', 'Javascript (For Beginner)', '2', 'ABC Supplier', '', 'Available');
/*!40000 ALTER TABLE `acession_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.acquisition_tbl: ~2 rows (approximately)
/*!40000 ALTER TABLE `acquisition_tbl` DISABLE KEYS */;
INSERT INTO `acquisition_tbl` (`ID`, `ISBN`, `Barcode`, `BookTitle`, `SupplierName`, `Donor`, `Quantity`, `BookPrice`, `TotalCost`, `TransactionNo`, `DateAcquired`) VALUES
	(1, '', '3860050162364', 'Ibong Adarna', 'ABC Supplier', '', '2', '123.00', '492.00', 'T-00001', '2026-03-05'),
	(2, '', '3831647888411', 'Javascript (For Beginner)', 'ABC Supplier', '', '5', '123.00', '738.00', 'T-00002', '2026-03-05');
/*!40000 ALTER TABLE `acquisition_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.audit_trail_tbl: ~173 rows (approximately)
/*!40000 ALTER TABLE `audit_trail_tbl` DISABLE KEYS */;
INSERT INTO `audit_trail_tbl` (`ID`, `Role`, `Email`, `ActionType`, `FormName`, `Description`, `DateTime`) VALUES
	(1, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.197.44.209.', '03/06/26-1:01 pm'),
	(2, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/06/26-1:07 pm'),
	(3, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.197.44.209.', '03/06/26-1:09 pm'),
	(4, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.30.84.209.', '03/06/26-1:14 pm'),
	(5, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 10.30.84.133.', '03/06/26-1:15 PM'),
	(6, 'Assistant Librarian', 'xzzu09@gmail.com', 'ADD', 'AUTHOR FORM', 'Added new Author: civ', '03/06/26-1:15 PM'),
	(7, 'Librarian', 'nazzrodin01@gmail.com', 'DELETE', 'AUTHOR FORM', 'Deleted Author: civ', '03/06/26-1:15 pm'),
	(8, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/06/26-1:15 PM'),
	(9, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 10.30.84.133.', '03/06/26-1:16 PM'),
	(10, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'ACQUISITION FORM', 'Updated Acquisition Record for Book: Ibong Adarna (ID: 1)', '03/06/26-1:17 pm'),
	(11, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/06/26-1:17 PM'),
	(12, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/06/26-1:17 pm'),
	(13, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.134.14.209.', '03/06/26-9:03 pm'),
	(14, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'ACQUISITION FORM', 'Updated Acquisition Record for Book: Javascript (For Beginner) (ID: 2)', '03/06/26-9:05 pm'),
	(15, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'ACQUISITION FORM', 'Updated Acquisition Record for Book: Javascript (For Beginner) (ID: 2)', '03/06/26-9:06 pm'),
	(16, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'ACQUISITION FORM', 'Updated Acquisition Record for Book: Javascript (For Beginner) (ID: 2)', '03/06/26-9:06 pm'),
	(17, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/06/26-9:09 pm'),
	(18, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.134.14.209.', '03/07/26-12:42 am'),
	(19, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/07/26-12:42 am'),
	(20, 'Staff', 'lizzjanellelerumaquino@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'staff\' (Staff) successfully logged in from IP: 10.134.14.209.', '03/07/26-12:42 am'),
	(21, 'Staff', 'lizzjanellelerumaquino@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'staff\' (Staff) successfully logged out.', '03/07/26-12:43 am'),
	(22, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 10.134.14.209.', '03/07/26-12:43 am'),
	(23, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/07/26-12:45 am'),
	(24, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-9:54 pm'),
	(25, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BORROWING FORM', 'Book \'Javascript (For Beginner)\' (AccessionID: 45500) added to transaction 260308215439 for borrower Myra Bayot B. [Change: N/A -> Borrower: Myra Bayot B (01234567), Book: Javascript (For Beginner), AccID: 45500]', '03/08/26-9:54 pm'),
	(26, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=False (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-9:55 pm'),
	(27, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-9:55 pm'),
	(28, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-9:57 pm'),
	(29, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-9:58 pm'),
	(30, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-9:58 pm'),
	(31, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-10:02 pm'),
	(32, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-10:28 pm'),
	(33, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-10:29 pm'),
	(34, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-10:30 pm'),
	(35, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-10:31 pm'),
	(36, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-10:31 pm'),
	(37, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-10:31 pm'),
	(38, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-10:42 pm'),
	(39, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-10:42 pm'),
	(40, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-10:43 pm'),
	(41, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-10:46 pm'),
	(42, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-10:46 pm'),
	(43, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-10:48 pm'),
	(44, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-10:55 pm'),
	(45, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-10:55 pm'),
	(46, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-10:56 pm'),
	(47, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-11:05 pm'),
	(48, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:06 pm'),
	(49, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:06 pm'),
	(50, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:06 pm'),
	(51, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:07 pm'),
	(52, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-11:09 pm'),
	(53, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-11:13 pm'),
	(54, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:13 pm'),
	(55, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-11:14 pm'),
	(56, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-11:17 pm'),
	(57, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:17 pm'),
	(58, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-11:20 pm'),
	(59, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-11:23 pm'),
	(60, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:24 pm'),
	(61, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-11:29 pm'),
	(62, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-11:32 pm'),
	(63, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:33 pm'),
	(64, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-11:33 pm'),
	(65, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-11:39 pm'),
	(66, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:40 pm'),
	(67, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-11:41 pm'),
	(68, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/08/26-11:47 pm'),
	(69, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/08/26-11:47 pm'),
	(70, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/08/26-11:49 pm'),
	(71, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-12:05 am'),
	(72, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:06 am'),
	(73, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-12:07 am'),
	(74, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-12:10 am'),
	(75, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:11 am'),
	(76, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:11 am'),
	(77, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-12:12 am'),
	(78, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-12:15 am'),
	(79, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:16 am'),
	(80, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:16 am'),
	(81, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:16 am'),
	(82, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-12:17 am'),
	(83, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-12:25 am'),
	(84, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:25 am'),
	(85, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-12:26 am'),
	(86, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-12:31 am'),
	(87, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:31 am'),
	(88, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-12:31 am'),
	(89, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-12:38 am'),
	(90, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:38 am'),
	(91, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-12:39 am'),
	(92, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-12:45 am'),
	(93, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:45 am'),
	(94, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-12:46 am'),
	(95, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-12:56 am'),
	(96, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-12:56 am'),
	(97, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-12:57 am'),
	(98, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-1:05 am'),
	(99, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:05 am'),
	(100, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-1:06 am'),
	(101, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-1:15 am'),
	(102, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:15 am'),
	(103, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:15 am'),
	(104, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:16 am'),
	(105, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:17 am'),
	(106, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:18 am'),
	(107, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:19 am'),
	(108, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-1:19 am'),
	(109, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-1:25 am'),
	(110, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:25 am'),
	(111, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:25 am'),
	(112, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:26 am'),
	(113, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-1:26 am'),
	(114, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-1:29 am'),
	(115, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:30 am'),
	(116, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:30 am'),
	(117, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-1:31 am'),
	(118, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-1:35 am'),
	(119, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:35 am'),
	(120, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-1:36 am'),
	(121, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-1:38 am'),
	(122, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:38 am'),
	(123, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-1:38 am'),
	(124, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-1:44 am'),
	(125, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:44 am'),
	(126, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-1:45 am'),
	(127, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-1:52 am'),
	(128, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:53 am'),
	(129, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-1:53 am'),
	(130, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/09/26-1:57 am'),
	(131, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:57 am'),
	(132, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:57 am'),
	(133, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:58 am'),
	(134, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260308215439. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/09/26-1:58 am'),
	(135, 'Librarian', 'nazzrodin01@gmail.com', 'ADD', 'BOOK RETURN', 'Returned 1 book(s) for transaction 260308215439. Status: Normal Return. [Change: Borrower: Myra Bayot B -> Returned Books: Javascript (For Beginner)]', '03/09/26-2:01 am'),
	(136, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/09/26-2:02 am'),
	(137, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.255.219.209.', '03/10/26-1:35 pm'),
	(138, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'TIME-IN/OUT FORM', 'Time-Out recorded for Teacher ID 01234567. Time In: 2026-03-08 21:54:42 [Change: Time In: 2026-03-08 21:54:42, Time Out: NULL -> Time In: 2026-03-08 21:54:42, Time Out: 2026-03-10 13:37:22]', '03/10/26-1:37 pm'),
	(139, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-1:41 pm'),
	(140, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.21.154.209.', '03/10/26-3:38 pm'),
	(141, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-3:42 pm'),
	(142, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 10.21.154.209.', '03/10/26-3:42 pm'),
	(143, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/10/26-3:43 pm'),
	(144, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.21.154.209.', '03/10/26-3:44 pm'),
	(145, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-3:44 pm'),
	(146, 'Staff', 'lizzjanellelerumaquino@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'staff\' (Staff) successfully logged in from IP: 10.21.154.209.', '03/10/26-3:44 pm'),
	(147, 'Staff', 'lizzjanellelerumaquino@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'staff\' (Staff) successfully logged out.', '03/10/26-3:45 pm'),
	(148, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.21.154.209.', '03/10/26-3:49 pm'),
	(149, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-3:49 pm'),
	(150, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.21.154.209.', '03/10/26-3:50 pm'),
	(151, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-3:50 pm'),
	(152, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 10.21.154.209.', '03/10/26-4:10 pm'),
	(153, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-4:10 pm'),
	(154, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-5:31 pm'),
	(155, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260310160928. [Change: IsPrinted=False (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/10/26-5:31 pm'),
	(156, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-5:31 pm'),
	(157, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-5:37 pm'),
	(158, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260310160928. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/10/26-5:37 pm'),
	(159, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260310160928. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/10/26-5:38 pm'),
	(160, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-5:38 pm'),
	(161, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-5:43 pm'),
	(162, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260310160928. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/10/26-5:43 pm'),
	(163, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-6:01 pm'),
	(164, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-6:04 pm'),
	(165, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260310160928. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/10/26-6:04 pm'),
	(166, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-6:05 pm'),
	(167, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-6:11 pm'),
	(168, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260310160928. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/10/26-6:12 pm'),
	(169, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-6:13 pm'),
	(170, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-6:16 pm'),
	(171, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260310160928. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/10/26-6:16 pm'),
	(172, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-6:17 pm'),
	(173, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-6:46 pm'),
	(174, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'PRINT RECEIPT', 'Printed receipt for transaction 260310160928. [Change: IsPrinted=True (Borrower: Myra Bayot B) -> IsPrinted=1 (Printed by: admin)]', '03/10/26-6:47 pm'),
	(175, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-6:47 pm'),
	(176, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-6:48 pm'),
	(177, 'Assistant Librarian', 'xzzu09@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'assistant\' (Assistant Librarian) successfully logged out.', '03/10/26-6:48 pm'),
	(178, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-9:18 pm'),
	(179, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-9:18 pm'),
	(180, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-9:39 pm'),
	(181, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-9:39 pm'),
	(182, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-9:48 pm'),
	(183, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-9:48 pm'),
	(184, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/10/26-9:58 pm'),
	(185, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/10/26-9:58 pm'),
	(186, 'Librarian', 'nazzrodin01@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'admin\' (Librarian) successfully logged in from IP: 192.168.100.16.', '03/11/26-12:09 am'),
	(187, 'Librarian', 'nazzrodin01@gmail.com', 'UPDATE', 'TIME-IN/OUT FORM', 'Time-Out recorded for  ID 01234567. Time In: 2026-03-10 15:47:05 [Change: Time In: 2026-03-10 15:47:05, Time Out: NULL -> Time In: 2026-03-10 15:47:05, Time Out: 2026-03-11 00:11:50]', '03/11/26-12:11 am'),
	(188, 'Librarian', 'nazzrodin01@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'admin\' (Librarian) successfully logged out.', '03/11/26-12:21 am');
/*!40000 ALTER TABLE `audit_trail_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.author_tbl: ~2 rows (approximately)
/*!40000 ALTER TABLE `author_tbl` DISABLE KEYS */;
INSERT INTO `author_tbl` (`ID`, `AuthorName`) VALUES
	(1, 'Kean Abesa'),
	(2, 'hahaha');
/*!40000 ALTER TABLE `author_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.available_tbl: ~9 rows (approximately)
/*!40000 ALTER TABLE `available_tbl` DISABLE KEYS */;
INSERT INTO `available_tbl` (`ID`, `ISBN`, `Barcode`, `AccessionID`, `BookTitle`, `Shelf`, `Status`) VALUES
	(43, NULL, '3860050162364', '51036', 'Ibong Adarna', '1', 'Available'),
	(44, NULL, '3860050162364', '97237', 'Ibong Adarna', '1', 'Available'),
	(45, NULL, '3860050162364', '27309', 'Ibong Adarna', '1', 'Available'),
	(46, NULL, '3860050162364', '34606', 'Ibong Adarna', '1', 'Available'),
	(48, NULL, '3831647888411', '53816', 'Javascript (For Beginner)', '2', 'Available'),
	(50, NULL, '3831647888411', '32467', 'Javascript (For Beginner)', '2', 'Available'),
	(53, '', '3831647888411', '45500', 'Javascript (For Beginner)', '2', 'Available'),
	(54, '', '3831647888411', '11814', 'Javascript (For Beginner)', '2', 'Available'),
	(55, '', '3831647888411', '74881', 'Javascript (For Beginner)', '2', 'Available');
/*!40000 ALTER TABLE `available_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.book_tbl: ~1 rows (approximately)
/*!40000 ALTER TABLE `book_tbl` DISABLE KEYS */;
INSERT INTO `book_tbl` (`ID`, `Barcode`, `ISBN`, `BookTitle`, `Author`, `Genre`, `Publisher`, `Language`, `YearPublished`) VALUES
	(4, '3860050162364', NULL, 'Ibong Adarna', 'Kean Abesa', 'Comedy', 'Kean Abesa', 'English', '2002'),
	(5, '3831647888411', NULL, 'Javascript (For Beginner)', 'hahaha', 'Comedy', 'Kean Abesa', 'English', '2012');
/*!40000 ALTER TABLE `book_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.borrowedview_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `borrowedview_tbl` DISABLE KEYS */;
/*!40000 ALTER TABLE `borrowedview_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.borroweredit_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `borroweredit_tbl` DISABLE KEYS */;
INSERT INTO `borroweredit_tbl` (`ID`, `LRN`, `EmployeeNo`, `Email`, `Username`, `Password`, `is_logged_in`, `CurrentIP`) VALUES
	(1, NULL, '01234567', 'zxraye@gmail.com', 'myra', 'myra@12345', 0, NULL);
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
	(1, 'Teacher', '', '3860050162364', '34606', 'Ibong Adarna', '1', NULL, '01234567', 'Myra Bayot B', '2026-03-03 00:00:00', '2026-03-10 00:00:00', '260303211328', 'Granted'),
	(2, 'Teacher', '', '3831647888411', '24111', 'Javascript (For Beginner)', '2', NULL, '01234567', 'Myra Bayot B', '2026-03-04 00:00:00', '2026-03-11 00:00:00', '260304145233', 'Granted'),
	(3, 'Teacher', '', '3831647888411', '45500', 'Javascript (For Beginner)', '2', NULL, '01234567', 'Myra Bayot B', '2026-03-08 00:00:00', '2026-03-15 00:00:00', '260308215439', 'Granted'),
	(4, 'Teacher', '', '3860050162364', '34606', 'Ibong Adarna', '1', NULL, '01234567', 'Myra Bayot B', '2026-03-10 00:00:00', '2026-03-17 00:00:00', '260310160928', 'Granted');
/*!40000 ALTER TABLE `borrowinghistory_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.borrowing_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `borrowing_tbl` DISABLE KEYS */;
INSERT INTO `borrowing_tbl` (`ID`, `Borrower`, `ISBN`, `Barcode`, `AccessionID`, `BookTitle`, `Shelf`, `LRN`, `EmployeeNo`, `Name`, `BorrowedDate`, `DueDate`, `TransactionReceipt`) VALUES
	(1, 'Teacher', '', '3860050162364', '34606', 'Ibong Adarna', '1', NULL, '01234567', 'Myra Bayot B', 'March-10-2026', '2026-03-17 00:00:00', '260310160928');
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

-- Dumping data for table laybsisu_dbs.oras_tbl: ~1 rows (approximately)
/*!40000 ALTER TABLE `oras_tbl` DISABLE KEYS */;
INSERT INTO `oras_tbl` (`ID`, `Borrower`, `Department`, `LRN`, `EmployeeNo`, `FirstName`, `LastName`, `MiddleInitial`, `Contact`, `GradeLevel`, `Section`, `Strand`, `TimeIn`, `TimeOut`) VALUES
	(1, 'Teacher', 'Junior High School', NULL, '01234567', 'Myra', 'Bayot', 'B', '09936545252', '', '', '', '2026-03-03 20:41:43', '2026-03-04 14:57:22'),
	(2, 'Teacher', 'Junior High School', NULL, '01234567', 'Myra', 'Bayot', 'B', '09936545252', '', '', '', '2026-03-08 21:54:42', '2026-03-10 13:37:22'),
	(3, NULL, NULL, NULL, '01234567', NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-03-10 15:47:05', '2026-03-11 00:11:50'),
	(4, NULL, NULL, NULL, '01234567', NULL, NULL, NULL, NULL, NULL, NULL, NULL, '2026-03-11 00:13:44', '2026-03-11 00:44:09');
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
INSERT INTO `penalty_tbl` (`ID`, `Borrower`, `LRN`, `EmployeeNo`, `FullName`, `Department`, `Grade`, `Section`, `Strand`, `ReturnedBook`, `BookTotal`, `BorrowedDate`, `DueDate`, `ReturnDate`, `TransactionReceipt`, `Status`, `BorrowerStatus`) VALUES
	(1, 'Teacher', NULL, '01234567', 'Myra Bayot B', 'Junior High School', NULL, NULL, NULL, 'Javascript (For Beginner)', '1', '3/4/2026', '3/11/2026', '3/4/2026', '260304145233', 'Damaged (Major)', 'PENALIZED');
/*!40000 ALTER TABLE `penalty_tbl` ENABLE KEYS */;

-- Dumping data for table laybsisu_dbs.printreceipt_tbl: ~0 rows (approximately)
/*!40000 ALTER TABLE `printreceipt_tbl` DISABLE KEYS */;
INSERT INTO `printreceipt_tbl` (`ID`, `Borrower`, `Name`, `BorrowedDate`, `DueDate`, `BorrowedBookCount`, `TransactionReceipt`, `IsPrinted`) VALUES
	(1, 'Teacher', 'Myra Bayot B', 'March-10-2026', '2026-03-17', '1', '260310160928', 1);
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
	(11, 'Teacher', NULL, '01234567', 'Myra Bayot B', 'Junior High School', NULL, NULL, NULL, 'Ibong Adarna', '1', '03/03/2026', '10/03/2026', '03/03/2026', '260303211328', 'Normal', 'N/A'),
	(12, 'Teacher', NULL, '01234567', 'Myra Bayot B', 'Junior High School', NULL, NULL, NULL, 'Javascript (For Beginner)', '1', '3/4/2026', '3/11/2026', '3/4/2026', '260304145233', 'Damaged (Major)', 'PENALIZED'),
	(13, 'Teacher', NULL, '01234567', 'Myra Bayot B', 'Junior High School', NULL, NULL, NULL, 'Javascript (For Beginner)', '1', '08/03/2026', '15/03/2026', '09/03/2026', '260308215439', 'Normal', 'N/A');
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
	(1, '0.0.0.0', 'Nazzer', 'Lindungan', 'K', 'assistant', 'assistant@123', 'xzzu09@gmail.com', 'Manila', '09242357456', 'Male', 'Assistant Librarian', 0),
	(2, '0.0.0.0', 'Earl John', 'Cejo', 'N/A', 'staff', 'staff@12345', 'lizzjanellelerumaquino@gmail.com', 'Badingan', '09437543454', 'Male', 'Staff', 0);
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
