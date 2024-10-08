﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using ShopProject.DataAccess.Data.Repository.IRepository;
using ShopProject.Models;
using ShopProject.Models.ViewModels;
using ShopProject.Utility;
using System.Diagnostics;
using System.Security.Claims;

namespace MVCProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty] public OrderVM OrderVm { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVm = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == orderId,
                    includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(od => od.OrderHeaderId == orderId,
                    includeProperties: nameof(Product))
            };
            return View(OrderVm);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(o => o.Id == OrderVm.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVm.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVm.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVm.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVm.OrderHeader.City;
            orderHeaderFromDb.State = OrderVm.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVm.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderVm.OrderHeader.Carrier))
                orderHeaderFromDb.Carrier = OrderVm.OrderHeader.Carrier;

            if (!string.IsNullOrEmpty(OrderVm.OrderHeader.TrackingNumber))
                orderHeaderFromDb.TrackingNumber = OrderVm.OrderHeader.TrackingNumber;

            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVm.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Updated Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVm.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVm.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVm.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            TempData["success"] = "Order Shipped Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVm.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }

            _unitOfWork.Save();

            TempData["success"] = "Order Canceled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = OrderVm.OrderHeader.Id });
        }

        [HttpPost]
        [ActionName("Details")]
        public IActionResult DetailsPayNow()
        {
            OrderVm.OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVm.OrderHeader.Id,
                includeProperties: nameof(ApplicationUser));
            OrderVm.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.Id == OrderVm.OrderHeader.Id,
                includeProperties: nameof(Product));

            _unitOfWork.OrderHeader.UpdateStatus(OrderVm.OrderHeader.Id, OrderVm.OrderHeader.OrderStatus, SD.PaymentStatusApproved);
            _unitOfWork.Save();

            return View();
        }


        #region API Calls

        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders;


            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(o => o.ApplicationUserId == userId,
                    includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(o => o.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }


            return Json(new { data = objOrderHeaders });
        }

        #endregion
    }
}