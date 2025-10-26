-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Oct 22, 2025 at 05:45 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `laybsisu_dbs`
--

-- --------------------------------------------------------

--
-- Table structure for table `acession_tbl`
--

CREATE TABLE `acession_tbl` (
  `ID` int(200) NOT NULL,
  `TransactionNo` varchar(40) NOT NULL,
  `AccessionID` varchar(25) NOT NULL,
  `ISBN` varchar(200) DEFAULT NULL,
  `Barcode` varchar(100) DEFAULT NULL,
  `BookTitle` varchar(100) NOT NULL,
  `Shelf` varchar(25) NOT NULL,
  `SupplierName` varchar(25) NOT NULL,
  `Status` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `acession_tbl`
--

INSERT INTO `acession_tbl` (`ID`, `TransactionNo`, `AccessionID`, `ISBN`, `Barcode`, `BookTitle`, `Shelf`, `SupplierName`, `Status`) VALUES
(1, 'T-00001', '99852', NULL, '8835268633791', 'Java', '1', 'Nazzrodin Lindungan', 'Available'),
(3, 'T-00001', '84539', NULL, '9085528960015', 'My father\'s died before i was born', '1', 'Jessica', 'Available'),
(9, 'T-00001', '34980', NULL, '8835268633791', 'Java', '1', 'Nazzrodin Lindungan', 'Available'),
(10, 'T-00001', '61705', NULL, '9085528960015', 'My father\'s died before i was born', '1', 'Jessica', 'Available');

-- --------------------------------------------------------

--
-- Table structure for table `acquisition_tbl`
--

CREATE TABLE `acquisition_tbl` (
  `ID` int(200) NOT NULL,
  `ISBN` varchar(100) DEFAULT NULL,
  `Barcode` varchar(100) DEFAULT NULL,
  `BookTitle` varchar(100) NOT NULL,
  `SupplierName` varchar(100) NOT NULL,
  `Quantity` varchar(100) NOT NULL,
  `BookPrice` varchar(100) NOT NULL,
  `TotalCost` varchar(100) NOT NULL,
  `TransactionNo` varchar(100) NOT NULL,
  `DateAcquired` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `acquisition_tbl`
--

INSERT INTO `acquisition_tbl` (`ID`, `ISBN`, `Barcode`, `BookTitle`, `SupplierName`, `Quantity`, `BookPrice`, `TotalCost`, `TransactionNo`, `DateAcquired`) VALUES
(49, '', '8835268633791', 'Java', 'Nazzrodin Lindungan', '2', '200', '600', 'T-00001', '2025-10-12'),
(50, '', '9085528960015', 'My father\'s died before i was born', 'Jessica', '2', '200', '600', 'T-00001', '2025-10-05');

-- --------------------------------------------------------

--
-- Table structure for table `audit_trail_tbl`
--

CREATE TABLE `audit_trail_tbl` (
  `ID` int(11) NOT NULL,
  `Role` varchar(100) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `ActionType` varchar(100) DEFAULT NULL,
  `FormName` varchar(100) DEFAULT NULL,
  `Description` varchar(1000) DEFAULT NULL,
  `DateTime` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `audit_trail_tbl`
--

INSERT INTO `audit_trail_tbl` (`ID`, `Role`, `Email`, `ActionType`, `FormName`, `Description`, `DateTime`) VALUES
(9, 'Librarian', 'nazz011@gmail.com', 'LOGIN SUCCESS', 'LOGIN FORM', 'User \'nazzrodin\' (Librarian) successfully logged in.', '10/22/25-11:42 pm'),
(10, 'Librarian', 'nazz011@gmail.com', 'UPDATE', 'TIME-IN/OUT FORM', 'Time-Out recorded for  ID 4231556. Time In: 2025-10-20 22:12:01 [Change: Time In: 2025-10-20 22:12:01, Time Out: NULL -> Time In: 2025-10-20 22:12:01, Time Out: 2025-10-22 23:42:20]', '10/22/25-11:42 pm'),
(11, 'Librarian', 'nazz011@gmail.com', 'UPDATE', 'TIME-IN/OUT FORM', 'Time-Out recorded for  ID 4231557. Time In: 2025-10-21 00:01:41 [Change: Time In: 2025-10-21 00:01:41, Time Out: NULL -> Time In: 2025-10-21 00:01:41, Time Out: 2025-10-22 23:42:23]', '10/22/25-11:42 pm'),
(12, 'Librarian', 'nazz011@gmail.com', 'UPDATE', 'RESERVE COPIES FORM', 'Pushed reserved copy (Acc. ID: 34980, Title: Java) back to Available status. [Change: Reserved -> Available]', '10/22/25-11:42 pm'),
(13, 'Librarian', 'nazz011@gmail.com', 'UPDATE', 'RESERVE COPIES FORM', 'Pushed reserved copy (Acc. ID: 61705, Title: My father\'s died before i was born) back to Available status. [Change: Reserved -> Available]', '10/22/25-11:42 pm'),
(14, 'Librarian', 'nazz011@gmail.com', 'LOGOUT SUCCESS', 'MAIN FORM', 'User \'nazzrodin\' (Librarian) successfully logged out.', '10/22/25-11:43 pm');

-- --------------------------------------------------------

--
-- Table structure for table `author_tbl`
--

CREATE TABLE `author_tbl` (
  `ID` int(100) NOT NULL,
  `AuthorName` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `author_tbl`
--

INSERT INTO `author_tbl` (`ID`, `AuthorName`) VALUES
(1, 'Kean'),
(2, 'Balindong'),
(4, 'Hazel P. Copiaco');

-- --------------------------------------------------------

--
-- Table structure for table `available_tbl`
--

CREATE TABLE `available_tbl` (
  `ID` int(200) NOT NULL,
  `ISBN` varchar(40) DEFAULT NULL,
  `Barcode` varchar(25) DEFAULT NULL,
  `AccessionID` varchar(100) NOT NULL,
  `BookTitle` varchar(200) NOT NULL,
  `Shelf` varchar(100) NOT NULL,
  `Status` varchar(25) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `available_tbl`
--

INSERT INTO `available_tbl` (`ID`, `ISBN`, `Barcode`, `AccessionID`, `BookTitle`, `Shelf`, `Status`) VALUES
(253, NULL, '8835268633791', '99852', 'Java', '1', 'Available'),
(255, NULL, '9085528960015', '84539', 'My father\'s died before i was born', '1', 'Available'),
(261, NULL, '8835268633791', '34980', 'Java', '1', 'Available'),
(262, NULL, '9085528960015', '61705', 'My father\'s died before i was born', '1', 'Available');

-- --------------------------------------------------------

--
-- Table structure for table `book_tbl`
--

CREATE TABLE `book_tbl` (
  `ID` int(200) NOT NULL,
  `Barcode` varchar(200) DEFAULT NULL,
  `ISBN` varchar(15) DEFAULT NULL,
  `BookTitle` varchar(55) NOT NULL,
  `Author` varchar(25) NOT NULL,
  `Genre` varchar(15) NOT NULL,
  `Category` varchar(15) NOT NULL,
  `Publisher` varchar(20) NOT NULL,
  `Language` varchar(15) NOT NULL,
  `YearPublished` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `book_tbl`
--

INSERT INTO `book_tbl` (`ID`, `Barcode`, `ISBN`, `BookTitle`, `Author`, `Genre`, `Category`, `Publisher`, `Language`, `YearPublished`) VALUES
(17, NULL, '9789710738663', 'Halinang Umawit at Gumuhit', 'Hazel P. Copiaco', 'Fantasy', 'Filipino', 'Earl John', 'Filipino', '2020-05-11'),
(18, '0400872653858', NULL, 'Ibong adarna', 'Balindong', 'Horror', 'Filipino', 'Makki', 'Filipino', '2020-05-11'),
(20, NULL, '9789710738946', 'Araling Panlipunan', 'Hazel P. Copiaco', 'Fantasy', 'Filipino', 'Earl John', 'Filipino', '2016-03-16'),
(21, '0491821303649', NULL, 'Javascript', 'Balindong', 'Fantasy', 'English', 'Makki', 'English', '2000-06-14'),
(22, '8999562339175', NULL, 'Kabataan', 'Hazel P. Copiaco', 'Fantasy', 'Filipino', 'Makki', 'English', '2023-09-24'),
(23, '5080148124663', NULL, 'Laravel & Laragon', 'Balindong', 'Horror', 'English', 'Makki', 'English', '2004-06-22'),
(24, '8835268633791', NULL, 'Java', 'Kean', 'Fantasy', 'English', 'Earl John', 'English', '2025-10-06'),
(25, '4442032773073', NULL, 'HAHAHA', 'Kean', 'Fantasy', 'English', 'Earl John', 'Filipino', '2025-10-06'),
(26, '1404206600928', NULL, 'Mobile Legends (Bang-bang)', 'Balindong', 'Fantasy', 'Math', 'Earl John', 'Filipino', '2023-09-25'),
(27, '5438429017699', NULL, 'My mother\'s pregnancy', 'Kean', 'SciFi', 'English', 'Earl John', 'Filipino', '2023-05-08'),
(28, '9085528960015', NULL, 'My father\'s died before i was born', 'Kean', 'Horror', 'Math', 'Earl John', 'English', '2009-11-26');

-- --------------------------------------------------------

--
-- Table structure for table `borrowedview_tbl`
--

CREATE TABLE `borrowedview_tbl` (
  `ID` int(200) NOT NULL,
  `BorrowerType` varchar(200) DEFAULT NULL,
  `FullName` varchar(200) DEFAULT NULL,
  `AccessionID` varchar(200) DEFAULT NULL,
  `BookTitle` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `borroweredit_tbl`
--

CREATE TABLE `borroweredit_tbl` (
  `ID` int(200) NOT NULL,
  `LRN` varchar(15) DEFAULT NULL,
  `EmployeeNo` varchar(15) DEFAULT NULL,
  `Email` varchar(100) NOT NULL,
  `Username` varchar(100) NOT NULL,
  `Password` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `borroweredit_tbl`
--

INSERT INTO `borroweredit_tbl` (`ID`, `LRN`, `EmployeeNo`, `Email`, `Username`, `Password`) VALUES
(14, '4231556', NULL, 'nazzu@gmail.com', 'nazz', '123');

-- --------------------------------------------------------

--
-- Table structure for table `borrower_tbl`
--

CREATE TABLE `borrower_tbl` (
  `ID` int(100) NOT NULL,
  `Borrower` varchar(15) NOT NULL,
  `FirstName` varchar(15) NOT NULL,
  `LastName` varchar(15) NOT NULL,
  `MiddleInitial` varchar(20) NOT NULL,
  `LRN` varchar(15) DEFAULT NULL,
  `EmployeeNo` varchar(25) DEFAULT NULL,
  `ContactNumber` varchar(11) NOT NULL,
  `Department` varchar(25) NOT NULL,
  `Grade` varchar(15) NOT NULL,
  `Section` varchar(25) NOT NULL,
  `Strand` varchar(25) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `borrower_tbl`
--

INSERT INTO `borrower_tbl` (`ID`, `Borrower`, `FirstName`, `LastName`, `MiddleInitial`, `LRN`, `EmployeeNo`, `ContactNumber`, `Department`, `Grade`, `Section`, `Strand`) VALUES
(17, 'Student', 'Nazzrodin', 'Lindungan', 'S.', '4231556', NULL, '09109224124', 'Senior High School', '12', '', 'ICT'),
(18, 'Student', 'Jessica', 'Dalinog', 'D', '4231557', NULL, '09143295675', 'Senior High School', '12', '', 'ICT');

-- --------------------------------------------------------

--
-- Table structure for table `borrowinghistory_tbl`
--

CREATE TABLE `borrowinghistory_tbl` (
  `ID` int(100) NOT NULL,
  `Borrower` varchar(100) NOT NULL,
  `ISBN` varchar(100) DEFAULT NULL,
  `Barcode` varchar(100) DEFAULT NULL,
  `AccessionID` varchar(100) NOT NULL,
  `BookTitle` varchar(100) NOT NULL,
  `Shelf` varchar(100) NOT NULL,
  `LRN` varchar(100) DEFAULT NULL,
  `EmployeeNo` varchar(100) DEFAULT NULL,
  `Name` varchar(100) NOT NULL,
  `BorrowedDate` varchar(100) NOT NULL,
  `DueDate` varchar(100) NOT NULL,
  `TransactionReceipt` varchar(100) NOT NULL,
  `Status` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `borrowing_tbl`
--

CREATE TABLE `borrowing_tbl` (
  `ID` int(100) NOT NULL,
  `Borrower` varchar(25) NOT NULL,
  `ISBN` varchar(100) DEFAULT NULL,
  `Barcode` varchar(100) DEFAULT NULL,
  `AccessionID` varchar(100) NOT NULL,
  `BookTitle` varchar(100) NOT NULL,
  `Shelf` varchar(100) NOT NULL,
  `LRN` varchar(100) DEFAULT NULL,
  `EmployeeNo` varchar(15) DEFAULT NULL,
  `Name` varchar(25) NOT NULL,
  `BorrowedDate` varchar(100) NOT NULL,
  `DueDate` varchar(100) NOT NULL,
  `TransactionReceipt` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `category_tbl`
--

CREATE TABLE `category_tbl` (
  `ID` int(100) NOT NULL,
  `Category` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `category_tbl`
--

INSERT INTO `category_tbl` (`ID`, `Category`) VALUES
(7, 'English'),
(8, 'Filipino'),
(9, 'Math');

-- --------------------------------------------------------

--
-- Table structure for table `confimation_tbl`
--

CREATE TABLE `confimation_tbl` (
  `ID` int(100) NOT NULL,
  `Borrower` varchar(100) NOT NULL,
  `Name` varchar(100) NOT NULL,
  `BorrowedDate` varchar(100) NOT NULL,
  `BorrowedBookCount` varchar(100) NOT NULL,
  `DaysLimit` varchar(100) NOT NULL,
  `DueDate` varchar(100) NOT NULL,
  `TransactionReceipt` varchar(100) NOT NULL,
  `Status` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `damagedview_tbl`
--

CREATE TABLE `damagedview_tbl` (
  `ID` int(200) NOT NULL,
  `AccessionID` varchar(200) DEFAULT NULL,
  `BookTitle` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `department_tbl`
--

CREATE TABLE `department_tbl` (
  `ID` int(100) NOT NULL,
  `Department` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `department_tbl`
--

INSERT INTO `department_tbl` (`ID`, `Department`) VALUES
(1, 'Junior High School'),
(2, 'Senior High School'),
(3, 'Elementary');

-- --------------------------------------------------------

--
-- Table structure for table `genre_tbl`
--

CREATE TABLE `genre_tbl` (
  `ID` int(100) NOT NULL,
  `Genre` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `genre_tbl`
--

INSERT INTO `genre_tbl` (`ID`, `Genre`) VALUES
(4, 'Fantasy'),
(5, 'SciFi'),
(6, 'Horror');

-- --------------------------------------------------------

--
-- Table structure for table `grade_tbl`
--

CREATE TABLE `grade_tbl` (
  `ID` int(200) NOT NULL,
  `Grade` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `grade_tbl`
--

INSERT INTO `grade_tbl` (`ID`, `Grade`) VALUES
(13, '1'),
(14, '2'),
(15, '3'),
(16, '4'),
(17, '5'),
(18, '6'),
(19, '7'),
(20, '8'),
(21, '9'),
(22, '10'),
(23, '11'),
(24, '12');

-- --------------------------------------------------------

--
-- Table structure for table `language_tbl`
--

CREATE TABLE `language_tbl` (
  `ID` int(100) NOT NULL,
  `Language` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `language_tbl`
--

INSERT INTO `language_tbl` (`ID`, `Language`) VALUES
(5, 'English'),
(6, 'Filipino');

-- --------------------------------------------------------

--
-- Table structure for table `lostview_tbl`
--

CREATE TABLE `lostview_tbl` (
  `ID` int(200) NOT NULL,
  `AccessionID` varchar(200) DEFAULT NULL,
  `BookTitle` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `oras_tbl`
--

CREATE TABLE `oras_tbl` (
  `ID` int(100) NOT NULL,
  `Borrower` varchar(15) NOT NULL,
  `Department` varchar(25) NOT NULL,
  `LRN` varchar(20) DEFAULT NULL,
  `EmployeeNo` varchar(25) DEFAULT NULL,
  `FirstName` varchar(15) NOT NULL,
  `LastName` varchar(15) NOT NULL,
  `MiddleInitial` varchar(15) NOT NULL,
  `Contact` varchar(11) NOT NULL,
  `GradeLevel` varchar(25) DEFAULT NULL,
  `Section` varchar(30) DEFAULT NULL,
  `Strand` varchar(30) DEFAULT NULL,
  `TimeIn` varchar(30) DEFAULT NULL,
  `TimeOut` varchar(30) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `oras_tbl`
--

INSERT INTO `oras_tbl` (`ID`, `Borrower`, `Department`, `LRN`, `EmployeeNo`, `FirstName`, `LastName`, `MiddleInitial`, `Contact`, `GradeLevel`, `Section`, `Strand`, `TimeIn`, `TimeOut`) VALUES
(137, '', '', '4231556', NULL, '', '', '', '', NULL, NULL, NULL, '2025-10-20 13:09:31', '2025-10-20 13:10:03'),
(138, '', '', '4231556', NULL, '', '', '', '', NULL, NULL, NULL, '2025-10-20 13:12:47', '2025-10-20 13:13:14'),
(139, '', '', '4231556', NULL, '', '', '', '', NULL, NULL, NULL, '2025-10-20 14:06:26', '2025-10-20 14:06:58'),
(140, '', '', '4231556', NULL, '', '', '', '', NULL, NULL, NULL, '2025-10-20 16:41:51', '2025-10-20 18:49:02'),
(141, '', '', '4231556', NULL, '', '', '', '', NULL, NULL, NULL, '2025-10-20 22:12:01', '2025-10-22 23:42:20'),
(142, '', '', '4231557', NULL, '', '', '', '', NULL, NULL, NULL, '2025-10-21 00:01:41', '2025-10-22 23:42:23');

-- --------------------------------------------------------

--
-- Table structure for table `overdueview_tbl`
--

CREATE TABLE `overdueview_tbl` (
  `ID` int(200) NOT NULL,
  `AccessionID` int(200) DEFAULT NULL,
  `BookTitle` int(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `penalty_management_tbl`
--

CREATE TABLE `penalty_management_tbl` (
  `ID` int(200) NOT NULL,
  `PenaltyType` varchar(40) NOT NULL,
  `Amount` varchar(100) NOT NULL,
  `Description` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `penalty_management_tbl`
--

INSERT INTO `penalty_management_tbl` (`ID`, `PenaltyType`, `Amount`, `Description`) VALUES
(1, 'Overdue', '10.00', 'Overdue fine per day'),
(2, 'Lost Book', '500', 'Standard fee for lost book'),
(3, 'Damaged Book - Minor', '100', 'Standard fee for minor'),
(6, 'Damaged Book - Major', '500', 'Standard fee for major'),
(7, 'Damaged Book - Irreparable', '600', 'Standard fee for irreparable');

-- --------------------------------------------------------

--
-- Table structure for table `penalty_tbl`
--

CREATE TABLE `penalty_tbl` (
  `ID` int(100) NOT NULL,
  `Borrower` varchar(100) NOT NULL,
  `LRN` varchar(100) DEFAULT NULL,
  `EmployeeNo` varchar(100) DEFAULT NULL,
  `FullName` varchar(100) NOT NULL,
  `Department` varchar(100) NOT NULL,
  `Grade` varchar(100) DEFAULT NULL,
  `Section` varchar(100) DEFAULT NULL,
  `Strand` varchar(100) DEFAULT NULL,
  `ReturnedBook` varchar(100) NOT NULL,
  `BookTotal` varchar(100) NOT NULL,
  `BorrowedDate` varchar(100) NOT NULL,
  `DueDate` varchar(100) NOT NULL,
  `ReturnDate` varchar(100) NOT NULL,
  `TransactionReceipt` varchar(100) NOT NULL,
  `Status` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `printreceipt_tbl`
--

CREATE TABLE `printreceipt_tbl` (
  `ID` int(200) NOT NULL,
  `Borrower` varchar(100) NOT NULL,
  `Name` varchar(200) NOT NULL,
  `BorrowedDate` varchar(100) NOT NULL,
  `DueDate` varchar(100) NOT NULL,
  `BorrowedBookCount` varchar(100) NOT NULL,
  `TransactionReceipt` varchar(100) NOT NULL,
  `IsPrinted` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `publisher_tbl`
--

CREATE TABLE `publisher_tbl` (
  `ID` int(200) NOT NULL,
  `PublisherName` varchar(30) NOT NULL,
  `Address` varchar(15) NOT NULL,
  `ContactNumber` varchar(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `publisher_tbl`
--

INSERT INTO `publisher_tbl` (`ID`, `PublisherName`, `Address`, `ContactNumber`) VALUES
(2, 'Earl John', 'taguig', '09344242422'),
(3, 'Makki', 'Taguig', '09321424234');

-- --------------------------------------------------------

--
-- Table structure for table `reservecopiess_tbl`
--

CREATE TABLE `reservecopiess_tbl` (
  `ID` int(200) NOT NULL,
  `TransactionNo` varchar(40) NOT NULL,
  `AccessionID` varchar(25) NOT NULL,
  `ISBN` varchar(200) DEFAULT NULL,
  `Barcode` varchar(100) DEFAULT NULL,
  `BookTitle` varchar(100) NOT NULL,
  `Shelf` varchar(25) NOT NULL,
  `SupplierName` varchar(100) NOT NULL,
  `Status` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `reserveview_tbl`
--

CREATE TABLE `reserveview_tbl` (
  `ID` int(200) NOT NULL,
  `AccessionID` varchar(200) DEFAULT NULL,
  `BookTitle` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `returnedview_tbl`
--

CREATE TABLE `returnedview_tbl` (
  `ID` int(200) NOT NULL,
  `AccessionID` int(200) DEFAULT NULL,
  `BookTitle` int(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `returning_tbl`
--

CREATE TABLE `returning_tbl` (
  `ID` int(200) NOT NULL,
  `Borrower` varchar(20) NOT NULL,
  `LRN` varchar(15) DEFAULT NULL,
  `EmployeeNo` varchar(15) DEFAULT NULL,
  `FullName` varchar(30) NOT NULL,
  `Department` varchar(100) DEFAULT NULL,
  `Grade` varchar(100) DEFAULT NULL,
  `Section` varchar(100) DEFAULT NULL,
  `Strand` varchar(100) DEFAULT NULL,
  `ReturnedBook` varchar(100) NOT NULL,
  `BookTotal` varchar(100) NOT NULL,
  `BorrowedDate` varchar(100) NOT NULL,
  `DueDate` varchar(100) NOT NULL,
  `ReturnDate` varchar(200) NOT NULL,
  `TransactionReceipt` varchar(200) NOT NULL,
  `Status` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `section_tbl`
--

CREATE TABLE `section_tbl` (
  `ID` int(200) NOT NULL,
  `Department` varchar(100) DEFAULT NULL,
  `GradeLevel` varchar(100) DEFAULT NULL,
  `Section` varchar(100) DEFAULT NULL,
  `Strand` varchar(15) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `section_tbl`
--

INSERT INTO `section_tbl` (`ID`, `Department`, `GradeLevel`, `Section`, `Strand`) VALUES
(14, 'Junior High School', '7', 'Appler', NULL),
(15, 'Elementary', '1', 'Makahiya', NULL),
(16, 'Senior High School', '11', NULL, 'GAS'),
(17, 'Senior High School', '12', NULL, 'ICT'),
(18, 'Junior High School', '7', 'Balindong', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `shelf_tbl`
--

CREATE TABLE `shelf_tbl` (
  `ID` int(200) NOT NULL,
  `Shelf` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `shelf_tbl`
--

INSERT INTO `shelf_tbl` (`ID`, `Shelf`) VALUES
(1, '1'),
(2, '2'),
(3, '3'),
(4, '4'),
(5, '5'),
(6, '6'),
(7, '7'),
(8, '8'),
(9, '9');

-- --------------------------------------------------------

--
-- Table structure for table `strand_tbl`
--

CREATE TABLE `strand_tbl` (
  `ID` int(200) NOT NULL,
  `Strand` varchar(20) NOT NULL,
  `Description` varchar(30) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `strand_tbl`
--

INSERT INTO `strand_tbl` (`ID`, `Strand`, `Description`) VALUES
(4, 'GAS', 'General Academic Strand'),
(5, 'ICT', 'Information Commucation Techno');

-- --------------------------------------------------------

--
-- Table structure for table `superadmin_tbl`
--

CREATE TABLE `superadmin_tbl` (
  `ID` int(200) NOT NULL,
  `FirstName` varchar(20) NOT NULL,
  `LastName` varchar(25) NOT NULL,
  `MiddleName` varchar(20) DEFAULT NULL,
  `ContactNumber` varchar(11) NOT NULL,
  `Email` varchar(25) NOT NULL,
  `Address` varchar(25) NOT NULL,
  `Username` varchar(30) NOT NULL,
  `Password` varchar(30) NOT NULL,
  `Gender` varchar(100) NOT NULL,
  `Role` varchar(25) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `superadmin_tbl`
--

INSERT INTO `superadmin_tbl` (`ID`, `FirstName`, `LastName`, `MiddleName`, `ContactNumber`, `Email`, `Address`, `Username`, `Password`, `Gender`, `Role`) VALUES
(15, 'Nazzrodin', 'Lindungan', 'Saydoquen', '09109224124', 'nazz011@gmail.com', '', 'nazzrodin', '12345', 'Male', 'Librarian');

-- --------------------------------------------------------

--
-- Table structure for table `supplier_tbl`
--

CREATE TABLE `supplier_tbl` (
  `ID` int(200) NOT NULL,
  `SupplierName` varchar(30) NOT NULL,
  `Address` varchar(15) NOT NULL,
  `ContactNumber` varchar(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `supplier_tbl`
--

INSERT INTO `supplier_tbl` (`ID`, `SupplierName`, `Address`, `ContactNumber`) VALUES
(3, 'Nazzrodin Lindungan', 'Taguig', '0934546542'),
(4, 'Balindongssss', 'Balindong', '09345678765'),
(5, 'Jessica', 'Taguig', '09443432156');

-- --------------------------------------------------------

--
-- Table structure for table `timeinoutrecord_tbl`
--

CREATE TABLE `timeinoutrecord_tbl` (
  `ID` int(200) NOT NULL,
  `Date` varchar(100) NOT NULL,
  `Borrower` varchar(100) NOT NULL,
  `FullName` varchar(100) NOT NULL,
  `TimeIn` varchar(100) NOT NULL,
  `TimeOut` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `totalbooksview_tbl`
--

CREATE TABLE `totalbooksview_tbl` (
  `ID` int(200) NOT NULL,
  `BookTitle` varchar(200) DEFAULT NULL,
  `Quantity` varchar(200) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `user_staff_tbl`
--

CREATE TABLE `user_staff_tbl` (
  `ID` int(200) NOT NULL,
  `FirstName` varchar(15) DEFAULT NULL,
  `LastName` varchar(15) DEFAULT NULL,
  `MiddleInitial` varchar(15) DEFAULT NULL,
  `Username` varchar(25) NOT NULL,
  `Password` varchar(25) NOT NULL,
  `Email` varchar(25) NOT NULL,
  `Address` varchar(100) DEFAULT NULL,
  `ContactNumber` varchar(11) NOT NULL,
  `Gender` varchar(15) NOT NULL,
  `Role` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `user_staff_tbl`
--

INSERT INTO `user_staff_tbl` (`ID`, `FirstName`, `LastName`, `MiddleInitial`, `Username`, `Password`, `Email`, `Address`, `ContactNumber`, `Gender`, `Role`) VALUES
(1, 'Kean', 'Abesa', 'N/A', 'kean', '123', 'kean@gmail.com', 'Taguig', '09456723456', 'Male', 'Assistant Librarian'),
(2, 'Earl', 'Cejo', 'D.', 'cejo', '1234', 'cejo@gmail.com', 'Taguig', '09345676433', 'Male', 'Staff'),
(3, 'Tan', 'Carb', 'K.', 'Tan', '123', 'tan@gmail.com', 'Arca South', '09961734094', 'Male', 'Staff'),
(4, 'Joan', 'Watson', 'N/A', 'Joandoe', '12345', 'Flythoon.ercdoc@ph.com', '', '09956489273', 'Female', 'Assistant Librarian');

-- --------------------------------------------------------

--
-- Table structure for table `varkuwd_tbl`
--

CREATE TABLE `varkuwd_tbl` (
  `ID` int(200) NOT NULL,
  `Barcode` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `varkuwd_tbl`
--

INSERT INTO `varkuwd_tbl` (`ID`, `Barcode`) VALUES
(1, '5597039377056'),
(2, '0831380485892'),
(3, '9797954434230'),
(4, '6947108079725');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `acession_tbl`
--
ALTER TABLE `acession_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `acquisition_tbl`
--
ALTER TABLE `acquisition_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `audit_trail_tbl`
--
ALTER TABLE `audit_trail_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `author_tbl`
--
ALTER TABLE `author_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `available_tbl`
--
ALTER TABLE `available_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `book_tbl`
--
ALTER TABLE `book_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `borrowedview_tbl`
--
ALTER TABLE `borrowedview_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `borroweredit_tbl`
--
ALTER TABLE `borroweredit_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `borrower_tbl`
--
ALTER TABLE `borrower_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `borrowinghistory_tbl`
--
ALTER TABLE `borrowinghistory_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `borrowing_tbl`
--
ALTER TABLE `borrowing_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `category_tbl`
--
ALTER TABLE `category_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `confimation_tbl`
--
ALTER TABLE `confimation_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `damagedview_tbl`
--
ALTER TABLE `damagedview_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `department_tbl`
--
ALTER TABLE `department_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `genre_tbl`
--
ALTER TABLE `genre_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `grade_tbl`
--
ALTER TABLE `grade_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `language_tbl`
--
ALTER TABLE `language_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `lostview_tbl`
--
ALTER TABLE `lostview_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `oras_tbl`
--
ALTER TABLE `oras_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `overdueview_tbl`
--
ALTER TABLE `overdueview_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `penalty_management_tbl`
--
ALTER TABLE `penalty_management_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `penalty_tbl`
--
ALTER TABLE `penalty_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `printreceipt_tbl`
--
ALTER TABLE `printreceipt_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `publisher_tbl`
--
ALTER TABLE `publisher_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `reservecopiess_tbl`
--
ALTER TABLE `reservecopiess_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `reserveview_tbl`
--
ALTER TABLE `reserveview_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `returnedview_tbl`
--
ALTER TABLE `returnedview_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `returning_tbl`
--
ALTER TABLE `returning_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `section_tbl`
--
ALTER TABLE `section_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `shelf_tbl`
--
ALTER TABLE `shelf_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `strand_tbl`
--
ALTER TABLE `strand_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `superadmin_tbl`
--
ALTER TABLE `superadmin_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `supplier_tbl`
--
ALTER TABLE `supplier_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `timeinoutrecord_tbl`
--
ALTER TABLE `timeinoutrecord_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `totalbooksview_tbl`
--
ALTER TABLE `totalbooksview_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `user_staff_tbl`
--
ALTER TABLE `user_staff_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `varkuwd_tbl`
--
ALTER TABLE `varkuwd_tbl`
  ADD PRIMARY KEY (`ID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `acession_tbl`
--
ALTER TABLE `acession_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `acquisition_tbl`
--
ALTER TABLE `acquisition_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT for table `audit_trail_tbl`
--
ALTER TABLE `audit_trail_tbl`
  MODIFY `ID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `author_tbl`
--
ALTER TABLE `author_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `available_tbl`
--
ALTER TABLE `available_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=263;

--
-- AUTO_INCREMENT for table `book_tbl`
--
ALTER TABLE `book_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=29;

--
-- AUTO_INCREMENT for table `borrowedview_tbl`
--
ALTER TABLE `borrowedview_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `borroweredit_tbl`
--
ALTER TABLE `borroweredit_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `borrower_tbl`
--
ALTER TABLE `borrower_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT for table `borrowinghistory_tbl`
--
ALTER TABLE `borrowinghistory_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=86;

--
-- AUTO_INCREMENT for table `borrowing_tbl`
--
ALTER TABLE `borrowing_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=228;

--
-- AUTO_INCREMENT for table `category_tbl`
--
ALTER TABLE `category_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=17;

--
-- AUTO_INCREMENT for table `confimation_tbl`
--
ALTER TABLE `confimation_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=48;

--
-- AUTO_INCREMENT for table `damagedview_tbl`
--
ALTER TABLE `damagedview_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `department_tbl`
--
ALTER TABLE `department_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `genre_tbl`
--
ALTER TABLE `genre_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `grade_tbl`
--
ALTER TABLE `grade_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT for table `language_tbl`
--
ALTER TABLE `language_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT for table `lostview_tbl`
--
ALTER TABLE `lostview_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `oras_tbl`
--
ALTER TABLE `oras_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=143;

--
-- AUTO_INCREMENT for table `overdueview_tbl`
--
ALTER TABLE `overdueview_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `penalty_management_tbl`
--
ALTER TABLE `penalty_management_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `penalty_tbl`
--
ALTER TABLE `penalty_tbl`
  MODIFY `ID` int(100) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=81;

--
-- AUTO_INCREMENT for table `printreceipt_tbl`
--
ALTER TABLE `printreceipt_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=87;

--
-- AUTO_INCREMENT for table `publisher_tbl`
--
ALTER TABLE `publisher_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT for table `reservecopiess_tbl`
--
ALTER TABLE `reservecopiess_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=51;

--
-- AUTO_INCREMENT for table `reserveview_tbl`
--
ALTER TABLE `reserveview_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `returnedview_tbl`
--
ALTER TABLE `returnedview_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `returning_tbl`
--
ALTER TABLE `returning_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=88;

--
-- AUTO_INCREMENT for table `section_tbl`
--
ALTER TABLE `section_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;

--
-- AUTO_INCREMENT for table `shelf_tbl`
--
ALTER TABLE `shelf_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT for table `strand_tbl`
--
ALTER TABLE `strand_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `superadmin_tbl`
--
ALTER TABLE `superadmin_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT for table `supplier_tbl`
--
ALTER TABLE `supplier_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `timeinoutrecord_tbl`
--
ALTER TABLE `timeinoutrecord_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `totalbooksview_tbl`
--
ALTER TABLE `totalbooksview_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `user_staff_tbl`
--
ALTER TABLE `user_staff_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `varkuwd_tbl`
--
ALTER TABLE `varkuwd_tbl`
  MODIFY `ID` int(200) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
