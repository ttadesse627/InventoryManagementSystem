
export const metadata = {
  title: '404 - Page Not Found',
};

export default function NotFound() {

  return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
        <div className="text-center max-w-lg">
          <div className="mb-8">
            <div className="inline-flex items-center justify-center w-32 h-32 bg-linear-to-br from-purple-100 to-pink-100 rounded-full mb-6">
              <svg className="w-16 h-16 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <h1 className="text-4xl font-bold text-gray-900 mb-4">404</h1>
            <h2 className="text-2xl font-semibold text-gray-700 mb-4">Page Not Found!</h2>
            <p className="text-gray-600 mb-8">
              Sorry, we couldn't find the page you're looking for. Please check the URL or try again later.
            </p>
          </div>
        </div>
      </div>
  )
}