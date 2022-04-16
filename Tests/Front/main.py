import os
from selenium import webdriver
from selenium.webdriver.common.by import By

def make_order(driver):
    page2 = driver.find_element(By.ID, "page_2")

    page2.click()
    customer_input = driver.find_element(By.ID, "CUST_NAME")
    customer_input.send_keys("Customer Test1")

    address_input = driver.find_element(By.ID, "ADDRESS")
    address_input.send_keys("1640 W Colfax Ave, Denver, CO 80204")

    order_submit = driver.find_element(By.ID, "ORDER_SUBMIT")
    order_submit.click()

def check_tracking(driver):
    page3 = driver.find_element(By.ID, "page_3")
    page3.click()

    home_btn = driver.find_element(By.ID, "home_btn")
    home_btn.click()
    drone_btn = driver.find_element(By.ID, "drone_btn")
    drone_btn.click()
    dest_btn = driver.find_element(By.ID, "dest_btn")
    dest_btn.click()
    home_icon = driver.find_element(By.ID, "home_icon")
    home_icon.click()

    clear_btn = driver.find_element(By.CLASS_NAME, "mt-4")
    clear_btn.click()

def main():
    os.environ['PATH'] += r"C:\Users\hdelg\OneDrive\Desktop\Capstone\proj_tests\chromedriver.exe"
    driver = webdriver.Chrome()
    driver.get('https://localhost:44364/')
    driver.implicitly_wait(30)
    drone_conn_btn = driver.find_element(By.XPATH, "//div[@class='connection-bar']")
    drone_conn_btn.click()

    # Checks the order put then the tracking page.
    make_order(driver)
    check_tracking(driver)

    page1 = driver.find_element(By.ID, "page_1")
    page1.click()

if __name__ == "__main__":
    main()